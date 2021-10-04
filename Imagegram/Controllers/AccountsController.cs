using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Helpers;
using Imagegram.Models;
using Imagegram.Models.ViewModels;
using Imagegram.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Imagegram.Controllers
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        private IRepository<Account> _accRepo;

        private IRepository<Post> _postRepo;

        private IRepository<Comment> _commentRepo;

        private IWebHostEnvironment _hostingEnvironment;

        public AccountsController(IRepository<Account> accRepo,
            IRepository<Post> postRepo,
            IRepository<Comment> commentRepo,
            IWebHostEnvironment hostEnvironment)
        {
            this._accRepo = accRepo;
            this._postRepo = postRepo;
            this._commentRepo = commentRepo;
            this._hostingEnvironment = hostEnvironment;
        }
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
              AuthHelper.ValidateToken(Request);
                
                var accounts = _accRepo.GetAll();
                return new OkObjectResult(accounts);
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
               AuthHelper.ValidateToken(Request);
                var account = _accRepo.GetById(id);
                if (account == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(account);
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody] AccountViewModel accountvm)
        {
            try
            {
                if (accountvm == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                Account account = new Account
                {
                    Name = accountvm.Name,
                    Password = EncryptDecryptHelper.Encrypt(accountvm.Password)
                };
                
                _accRepo.Add(account);
                return new OkResult();
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordViewModel pwdvm)
        {
            try
            {

                AuthHelper.ValidateToken(Request);
                if (pwdvm == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                if (pwdvm.Id != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult("Permission denied. Only account owner is allowed. ");
                }
                Account account =  _accRepo.GetById(pwdvm.Id);
                if (account.Password == EncryptDecryptHelper.Encrypt(pwdvm.OldPassword))
                {
                    account.Password = EncryptDecryptHelper.Encrypt(pwdvm.NewPassword);
                }
                else
                {
                    return new BadRequestObjectResult("Wrong old password");
                }

                _accRepo.Update(account);
                return new OkResult();
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
        // PUT api/values/5
        [HttpPut]
        public IActionResult Put([FromBody] Account account)
        {
            try
            {
               AuthHelper.ValidateToken(Request);
                if (account.Id != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult("Permission denied. Only account owner is allowed. ");
                }
                if (account == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                _accRepo.UpdateWithExceptions(account, new[] { "Password" });
                return new OkResult();
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                AuthHelper.ValidateToken(Request);

                if (id != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult("Permission denied. Only account owner is allowed. ");
                }

                var account = _accRepo.Get(x => x.Id == id, null,"Posts,Comments").FirstOrDefault();

                if (account == null)
                {
                    return new NotFoundResult();
                }
                _commentRepo.DeleteAll(account.Comments);
                _postRepo.DeleteAll(account.Posts);
                _accRepo.Delete(account);
                return new OkResult();
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenValidationException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login([FromBody] AccountViewModel login)
        {
            try
            {
                if (login == null || login.Name == null || login.Password == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
               string encPassword = EncryptDecryptHelper.Encrypt(login.Password);
               Account account = _accRepo.Get(x => x.Name == login.Name && x.Password == encPassword).FirstOrDefault();
                if (account == null)
                {
                    return new BadRequestObjectResult("User name or password is incorrect.");
                }
                var token = AuthHelper.GenerateToken(account);
                if (token == null)
                    return Unauthorized();
                return new OkObjectResult( new { token, imagePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Images/" ) });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Models;
using Imagegram.Repositories;
using Imagegram.Helpers;
using Microsoft.AspNetCore.Mvc;
using Imagegram.Models.ViewModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Imagegram.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : Controller
    {
        private IRepository<Comment> _commentRepo;

        public CommentsController(IRepository<Comment> commentRepo)
        {
            this._commentRepo = commentRepo;
        }
        // GET: api/values
        [HttpGet]
        public IActionResult Get(int? postId)
        {
            try
            {
                AuthHelper.ValidateToken(Request);

                if (!postId.HasValue)
                {
                    return new BadRequestObjectResult("invalid request.");
                }
                var comments = _commentRepo.GetQueryable().Select(c => new CommentViewModel
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatorId = c.CreatorId,
                    PostId = c.PostId,
                    CreatorName = c.Creator.Name,
                    CreatedAt = c.CreatedAt
                }).Where(x => x.PostId == postId);
                return new OkObjectResult(comments);
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
                var comment = _commentRepo.GetById(id);
                if (comment == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(comment);
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
        public IActionResult Comment([FromBody] Comment comment)
        {
            try
            {
                AuthHelper.ValidateToken(Request);
                if (comment == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                comment.CreatorId = AuthHelper.logged_in_userid;
                comment.CreatedAt = DateTime.UtcNow;
                _commentRepo.Add(comment);
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
        public IActionResult Put([FromBody] Comment comment)
        {
            try
            {
                AuthHelper.ValidateToken(Request);
                if (comment == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                if (comment.CreatorId != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult("Permission denied. Only creator is allowed. ");
                }
                _commentRepo.Update(comment);
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
                var comment = _commentRepo.GetById(id);
                if (comment == null)
                {
                    return new NotFoundResult();
                }
                if (comment.CreatorId != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult( "Permission denied. Only creator is allowed. ");
                }
                _commentRepo.Delete(comment);
                return new OkResult();
            }
            catch (UnauthorizedAccessException)
            {
                return new UnauthorizedResult();
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(ex.Message);
            }
        }
    }
}

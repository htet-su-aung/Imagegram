using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Imagegram.Models;
using Imagegram.Repositories;
using Imagegram.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Hosting;
using Imagegram.Models.ViewModels;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Imagegram.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private IRepository<Post> _postRepo;
        private IWebHostEnvironment _hostingEnvironment;
        private IRepository<Comment> _commentRepo;

        public PostsController(IRepository<Post> postRepo,
            IRepository<Comment> commentRepo,
            IWebHostEnvironment hostingEnvironment)
        {
            this._postRepo = postRepo;
            this._commentRepo = commentRepo;
            this._hostingEnvironment = hostingEnvironment;

        }
        // GET: api/values
        [HttpGet]
        public IActionResult Get(int? prev = null, int? next = null, int? count = null)
        {
            try
            {
               
                AuthHelper.ValidateToken(Request);
                int rowcount = count.HasValue ? (int)count : Convert.ToInt32(ConfigHelper.GetValue("RowCount"));
                List<PostViewModel> posts = new List<PostViewModel>();
                Cursor cursor = new Cursor();
                if (next.HasValue)
                {
                    posts = _postRepo.GetQueryable().Select(p => new PostViewModel
                    {
                        Id = p.Id,
                        Caption = p.Caption,
                        CreatorId = p.CreatorId,
                        CreatorName = p.Creator.Name,
                        Image = p.Image,
                        CreatedAt = p.CreatedAt,
                        Comments = p.Comments.OrderByDescending(x => x.CreatedAt).Take(2).ToList(),
                        CommentCount = p.Comments.Count
                    })
                        .Where(x => x.CommentCount < next)
                        .OrderByDescending(x => x.CommentCount).ThenByDescending(x => x.Id)
                        .Take(rowcount)
                        .ToList();


                    cursor.Prev = posts.First().CommentCount;
                    cursor.Next = posts.Last().CommentCount;
                }
                else if(prev.HasValue)
                {
                    posts = _postRepo.GetQueryable().Select(p => new PostViewModel
                    {
                        Id = p.Id,
                        Caption = p.Caption,
                        CreatorId = p.CreatorId,
                        CreatorName = p.Creator.Name,
                        Image = p.Image,
                        CreatedAt = p.CreatedAt,
                        Comments = p.Comments.OrderByDescending(x => x.CreatedAt).Take(2).ToList(),
                        CommentCount = p.Comments.Count
                       
                    })
                         .Where(x => x.CommentCount > prev)
                        .OrderByDescending(x => x.CommentCount).ThenByDescending(x => x.Id)
                        .Take(rowcount)
                        .ToList();

                    cursor.Prev = posts.First().CommentCount;
                    cursor.Next = posts.Last().CommentCount;
                }
                else
                {
                    posts = _postRepo.GetQueryable().Select(p => new PostViewModel
                    {
                        Id = p.Id,
                        Caption = p.Caption,
                        CreatorId = p.CreatorId,
                        CreatorName = p.Creator.Name,
                        Image =  p.Image,
                        CreatedAt = p.CreatedAt,
                        Comments = p.Comments.OrderByDescending(x => x.CreatedAt).Take(2).ToList(),
                        CommentCount = p.Comments.Count
                    })
                        .OrderByDescending(x => x.CommentCount).ThenByDescending(x => x.Id)
                        .Take(rowcount)
                        .ToList();

                    cursor.Prev = null;
                    cursor.Next = posts.Last().CommentCount;
                }
                return new OkObjectResult( new { posts, cursor });
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
                var post = _postRepo.GetById(id);
                if (post == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(post);
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
        [HttpPost]
        public IActionResult Post([FromBody] Post post)
        {
            try
            {
                AuthHelper.ValidateToken(Request);
                if (post == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                post.CreatorId = AuthHelper.logged_in_userid;
                post.CreatedAt = DateTime.UtcNow;
                _postRepo.Add(post);
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
        // POST api/values
        [HttpPost("UploadPhoto")]

        public IActionResult UploadPhoto()
        {
            try
            {
                AuthHelper.ValidateToken(Request);
                var file = Request.Form.Files[0];
                var name = Guid.NewGuid().ToString() + ".jpg";
                Console.WriteLine(_hostingEnvironment.WebRootPath);
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, "Images/");

                using (var stream = new MemoryStream())
                {
                    file.CopyToAsync(stream).Wait();
                    using var img = Image.FromStream(stream);
                    img.Save(path + @"/" + name, ImageFormat.Jpeg);
                    img.Dispose();
                    stream.Dispose();
                }

                return new OkObjectResult(name);
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
        public IActionResult Put([FromBody] Post post)
        {
            try
            {
                 AuthHelper.ValidateToken(Request);
                if (post == null)
                {
                    return new BadRequestObjectResult("Empty request.");
                }
                if (post.CreatorId != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult("Permission denied. Only creator is allowed. ");
                }
                _postRepo.Update(post);
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
                var post = _postRepo.Get(x => x.Id == id, null, "Comments").FirstOrDefault();
                if (post == null)
                {
                    return new NotFoundResult();
                }
                if (post.CreatorId != AuthHelper.logged_in_userid)
                {
                    return new UnauthorizedObjectResult( "Permission denied. Only creator is allowed. ");
                }
                _commentRepo.DeleteAll(post.Comments);
                _postRepo.Delete(post);
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

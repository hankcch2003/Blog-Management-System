using BlogManagement.Api.DTOs.Posts;
using BlogManagement.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogManagement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        // GET: api/posts
        // 取得所有文章
        [HttpGet]
        public async Task<ActionResult<List<PostResponseDto>>> GetAll()
        {
            var posts = await _postService.GetAllAsync();

            return Ok(posts);
        }

        // GET: api/posts/5
        // 依照 ID 取得單篇文章
        [HttpGet("{id}")]
        public async Task<ActionResult<PostResponseDto>> GetById(int id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound(new
                {
                    message = "找不到指定的文章。"
                });
            }

            return Ok(post);
        }

        // POST: api/posts
        // 新增文章
        [HttpPost]
        public async Task<ActionResult<PostResponseDto>> Create(
            CreatePostDto dto)
        {
            try
            {
                var post = await _postService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = post.Id },
                    post
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/posts/5
        // 修改文章
        [HttpPut("{id}")]
        public async Task<ActionResult<PostResponseDto>> Update(
            int id,
            UpdatePostDto dto)
        {
            try
            {
                var post = await _postService.UpdateAsync(id, dto);

                if (post == null)
                {
                    return NotFound(new
                    {
                        message = "找不到指定的文章。"
                    });
                }

                return Ok(post);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/posts/5
        // 刪除文章
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _postService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "找不到指定的文章。"
                });
            }

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TODOService.Dtos;
using TODOService.Helpers;
using TODOService.Models;
using Task = TODOService.Models.Task;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TODOService.Controllers
{
    [Authorize]
    [EnableCors("appCors")]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TodoAppDatabaseContext _context;
        private IWebHostEnvironment _env;
        private FileSaver _fileSaver;
        public TasksController(TodoAppDatabaseContext _context, IWebHostEnvironment env)
        {
            this._context = _context;
            _env = env;
            _fileSaver = new FileSaver(_env);

        }

        [HttpPost]
        [Route("addTask")]
        public async Task<IActionResult> createUserTask([FromBody] TaskDto task)
        {

            try
            {
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if(task == null)
                {
                    return BadRequest("You can not create an invalid task");
                }
                if(userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }

                var newTask = new Task
                {
                    TaskTitle = task.TaskTitle,
                    UserId = userID,
                    TaskDescription = task.TaskDescription,
                    CreatedDate = DateTime.Now,
                    ScheduledDate = task.ScheduledDate != null ? task.ScheduledDate : null,
                    CompletedDate = task.CompletedDate != null ? task.CompletedDate : null
                };
                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();

                return Ok("You have successfully created a new task");
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("updateTask")]
        public async Task<IActionResult> updateUserTask([FromBody] TaskDto task)
        {
            try
            {
                // get current user ID 
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));

                if (task == null || task.TaskId == null || task.TaskId <=0)
                {
                    return BadRequest("You can not create an invalid task");
                }
                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }
                var dbTask = _context.Tasks.Where(t => t.TaskId == task.TaskId && t.UserId == userID).FirstOrDefault();
                if (dbTask == null)
                {
                    return BadRequest("You can not update an non-existing task");
                }

                dbTask.TaskTitle = task.TaskTitle;
                dbTask.TaskDescription = task.TaskDescription;
                dbTask.ScheduledDate = task.ScheduledDate != null ? task.ScheduledDate : null;
                dbTask.CompletedDate = task.CompletedDate != null ? task.CompletedDate : null;

                _context.Entry(dbTask).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("You have successfully updated a task");
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getAllUserTasks")]
        public async Task<IActionResult> getAllUserTask()
        {
            try
            {
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));
                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }

                var taskList = _context.Tasks.Select( t => new
                {
                    t.TaskId,
                    t.UserId,
                    Title = t.TaskTitle,
                    Description = t.TaskDescription,
                    t.CreatedDate,
                    t.ScheduledDate,
                    t.CompletedDate
                }).Where(t => t.UserId == userID).ToList();

                return Ok(taskList);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("getUserTaskByTaskId/{id}")]
        public async Task<IActionResult> getUserTaskByTaskId(int id)
        {
            try
            {
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));
                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }
                if(id ==null || id <= 0)
                {
                    return BadRequest("This task does not exist");
                }
                var task = _context.Tasks.Select(t => new
                {
                    t.TaskId,
                    t.UserId,
                    t.TaskTitle,
                    t.TaskDescription,
                    t.CreatedDate,
                    t.ScheduledDate,
                    t.CompletedDate
                }).Where(t => t.UserId == userID && t.TaskId == id).FirstOrDefault();

                return Ok(task);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        [HttpDelete]
        [Route("deleteUserTaskByTaskId/{id}")]
        public async Task<IActionResult> deleteUserTaskByTaskId(int id)
        {
            try
            {
                int userID = Convert.ToInt32(HttpContext.User.FindFirstValue("userID"));
                if (userID == null || userID <= 0)
                {
                    return BadRequest("This user is not allowed to use this endpoint");
                }
                if (id == null || id <= 0)
                {
                    return BadRequest("Invalid task ID");
                }
                var task = _context.Tasks.Where(t => t.UserId == userID && t.TaskId == id).FirstOrDefault();

                if(task == null)
                {
                    return BadRequest("You can not delete a task that does not exist");
                }
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
                return Ok("You have successfuly deleted a task");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

       
        [HttpPost("Upload")]
        public async Task<IActionResult> uploadFile([FromForm] IFormFile file)
        {
            try{
                // call file saver function
                _fileSaver.FileSaveAsync(file, "assets/images");
                return Ok("You have uploaded yyour file successfully");
            }catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        
    }
}

using IKF_Pranay_Task.Contextfile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IKF_Pranay_Task.Models;
using Microsoft.Data.SqlClient;

namespace IKF_Pranay_Task.Controllers
{
    public class UserController : Controller
    {
        private readonly MyNewContext _context;
        public UserController(MyNewContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> ListUsers()
        {
            var users = await _context.Users
                .FromSqlRaw("EXEC sp_GetAllUsers")  
                .ToListAsync();

            return View(users);  
        }
        public async Task<IActionResult> AddUsers()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUsers(User user)
        {
            var parameters = new[]
               {
                   new SqlParameter("@Name", user.Name),
                    new SqlParameter("@DOB", user.DOB),
                    new SqlParameter("@Designation", user.Designation),
                    new SqlParameter("@Skills", user.Skills)
                };

            await _context.Database.ExecuteSqlRawAsync("EXEC sp_CreateUser @Name, @DOB, @Designation, @Skills", parameters);

            return RedirectToAction(nameof(ListUsers));
            
        }



        [HttpGet]
        public async Task<IActionResult> EditUser(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID.");
            }

            var parameter = new SqlParameter("@UserId", id);
            var user = _context.Users
                .FromSqlRaw("EXEC sp_GetUserById @UserId", parameter).AsEnumerable()
                .FirstOrDefault();
            var UserId = user.UserId;

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest("User ID mismatch.");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                var parameters = new[]
                {
            new SqlParameter("@UserId", user.UserId),
            new SqlParameter("@Name", user.Name),
            new SqlParameter("@DOB", user.DOB),
            new SqlParameter("@Designation", user.Designation),
            new SqlParameter("@Skills",user.Skills)
        };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_UpdateUser @UserId, @Name, @DOB, @Designation, @Skills", parameters);

                return RedirectToAction(nameof(ListUsers));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "error occurred while updating the user. Please try again.");
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID.");
            }

            var parameter = new SqlParameter("@UserId", id);
            var user = _context.Users
                .FromSqlRaw("EXEC sp_GetUserById @UserId", parameter)
                .AsEnumerable()
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid User ID.");
            }

            try
            {
                var parameter = new SqlParameter("@UserId", id);
                await _context.Database.ExecuteSqlRawAsync("EXEC sp_DeleteUser @UserId", parameter);

                return RedirectToAction(nameof(ListUsers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "error occurred while deleting the user. Please try again.");
            }
        }
    }
}

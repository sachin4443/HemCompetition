using System.Security.Claims;
using HemCompetition.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HemCompetition.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AuthDbContext _authContext;

        public DashboardController(AuthDbContext authContext)
        {
            _authContext = authContext;
        }

        [HttpGet("user-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            // ==========================================
            // GET USER ID FROM JWT
            // ==========================================

            var userIdClaim = User.FindFirstValue("UserId");

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized(new
                {
                    message = "Invalid Token"
                });
            }

            if (!long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new
                {
                    message = "Invalid UserId"
                });
            }

            // ==========================================
            // GET USER PROFILE FROM POSTGRESQL
            // ==========================================

            var authUser = await _authContext.AuthUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (authUser == null)
            {
                return NotFound(new
                {
                    message = "User not found in auth_users."
                });
            }

            // ==========================================
            // RETURN DASHBOARD PROFILE
            // ==========================================

            return Ok(new
            {
                userId = authUser.UserId,

                firstName = authUser.FirstName,

                lastName = authUser.LastName,

                email = authUser.Email ?? authUser.EmailId,

                mobileNo = authUser.PhoneNumber,

                address = authUser.AddressLine1,

                city = authUser.City,

                pincode = authUser.Pincode,

                games = new object[]
                {
                    new
                    {
                        gameId = 1,
                        title = "Number Game",
                        status = "Start",
                        route = "/game/number"
                    },

                    new
                    {
                        gameId = 2,
                        title = "Quiz Game",
                        status = "Completed",
                        route = "/game/quiz"
                    },

                    new
                    {
                        gameId = 3,
                        title = "Word Game",
                        status = "Start",
                        route = "/game/word"
                    }
                }
            });
        }
    }
}

//using System.Security.Claims;
//using HemCompetition.API.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace HemCompetition.API.Controllers
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class DashboardController : ControllerBase
//    {
//        private readonly Hemvirtues2627Context _context;

//        public DashboardController(Hemvirtues2627Context context)
//        {
//            _context = context;
//        }

//        [HttpGet("user-profile")]
//        public async Task<IActionResult> GetUserProfile()
//        {
//            var aspNetUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

//            if (string.IsNullOrEmpty(aspNetUserId))
//            {
//                return Unauthorized(new { message = "Invalid Token" });
//            }

//            var userProfile = await _context.Users
//                .FirstOrDefaultAsync(u => u.AspNetUserId == aspNetUserId);

//            if (userProfile == null)
//            {
//                return NotFound(new { message = "User not found." });
//            }

//            var aspNetUser = await _context.AspNetUsers
//                .FirstOrDefaultAsync(u => u.Id == aspNetUserId);

//            return Ok(new
//            {
//                userId = userProfile.UserId,
//                firstName = userProfile.FirstName,
//                lastName = userProfile.LastName,
//                email = userProfile.EmailId ?? aspNetUser?.Email,
//                mobileNo = userProfile.MobileNo,
//                address = userProfile.AddressLine1,
//                city = userProfile.City,
//                pincode = userProfile.Pincode,
//                games = new object[]
//                {
//                    new { gameId = 1, title = "Number Game", status = "Start", route = "/game/number" },
//                    new { gameId = 2, title = "Quiz Game", status = "Completed", route = "/game/quiz" },
//                    new { gameId = 3, title = "Word Game", status = "Start", route = "/game/word" }
//                }
//            });
//        }
//    }
//}
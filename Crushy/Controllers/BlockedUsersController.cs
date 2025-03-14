﻿using Crushy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Crushy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BlockedUsersController : ControllerBase
    {
        private readonly BlockedUserService _blockedUserService;

        public BlockedUsersController(BlockedUserService blockedUserService)
        {
            _blockedUserService = blockedUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlockedUsers()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var blockedUsers = await _blockedUserService.GetBlockedUsersAsync(userId);
            return Ok(blockedUsers);
        }


        [HttpPost("{blockedUserId}")]
        public async Task<IActionResult> BlockUser(int blockedUserId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            if (userId == blockedUserId)
            {
                return BadRequest("Kendinizi engelleyemezsiniz.");
            }

            try
            {
                var blockedUser = await _blockedUserService.BlockUserAsync(userId, blockedUserId);
                return Ok(blockedUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{blockedUserId}")]
        public async Task<IActionResult> UnblockUser(int blockedUserId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                await _blockedUserService.UnblockUserAsync(userId, blockedUserId);
                return Ok(new { message = "Engel başarıyla kaldırıldı." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("check/{blockedUserId}")]
        public async Task<IActionResult> CheckIfBlocked(int blockedUserId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var isBlocked = await _blockedUserService.IsUserBlockedAsync(userId, blockedUserId);
            return Ok(new { isBlocked });
        }
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using finsyncapi.BAL.IServices;
using finsyncapi.DAL.IRepositories;
using finsyncapi.Dto;
using finsyncapi.Helper;
using finsyncapi.Models;

namespace finsyncapi.BAL.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<ResultDto<SnowFlakeId>> CreateGroupAsync(UserContext currentUser, GroupCreateDto req)
        {
            return await _groupRepository.CreateGroupAsync(currentUser.UserId, currentUser.ProfileId!.Value, req);
        }

        public async Task<ResultDto<bool>> JoinGroupAsync(UserContext currentUser, GroupJoinDto req)
        {
            return await _groupRepository.JoinGroupAsync(currentUser.UserId, currentUser.ProfileId!.Value, req);
        }

        public async Task<ResultDto<IEnumerable<GroupDto>>> GetGroupsByProfileAsync(UserContext currentUser)
        {
            return await _groupRepository.GetGroupsByProfileAsync(currentUser.ProfileId!.Value);
        }
    }
}

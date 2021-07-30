﻿using AutoMapper;
using TaskoMask.Application.Core.Helpers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaskoMask.Application.Users.Queries.Models;
using TaskoMask.Application.Core.Dtos.Users;
using TaskoMask.Domain.Data;
using TaskoMask.Domain.Entities;
using TaskoMask.Application.Core.Queries;

namespace TaskoMask.Application.Users.Queries.Handlers
{
    public class UsersQueryHandlers : BaseQueryHandler,
        IRequestHandler<GetUserByIdQuery, UserOutput>,
        IRequestHandler<GetUsersCountQuery, long>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public UsersQueryHandlers(IMapper mapper, UserManager<User> userManager, IMediator mediator) : base(mediator, mapper)
        {
            _userManager = userManager;
        }

        public async Task<UserOutput> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Id);
            return _mapper.Map<UserOutput>(user);
        }


        public async Task<long> Handle(GetUsersCountQuery request, CancellationToken cancellationToken)
        {
            return _userManager.Users.Count();
        }

    }
}

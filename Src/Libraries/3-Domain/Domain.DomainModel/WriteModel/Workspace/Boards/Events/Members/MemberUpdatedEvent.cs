﻿using TaskoMask.Domain.Core.Events;
using TaskoMask.Domain.Share.Enums;
using TaskoMask.Domain.WriteModel.Workspace.Boards.Entities;

namespace TaskoMask.Domain.WriteModel.Workspace.Boards.Events.Members
{
    public class MemberUpdatedEvent : DomainEvent
    {
        public MemberUpdatedEvent(string id, BoardMemberAccessLevel accessLevel) : base(entityId: id, entityType: nameof(Member))
        {
            Id = id;
            AccessLevel = accessLevel;
        }

        public string Id { get; }
        public BoardMemberAccessLevel AccessLevel { get;  }
    }
}
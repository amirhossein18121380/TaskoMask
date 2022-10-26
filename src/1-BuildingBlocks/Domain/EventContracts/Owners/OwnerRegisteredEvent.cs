﻿using TaskoMask.BuildingBlocks.Domain.Models;
using TaskoMask.BuildingBlocks.Domain.Resources;

namespace TaskoMask.BuildingBlocks.Domain.EventContracts.Owners
{
    public class OwnerRegisteredEvent : DomainEvent
    {
        public OwnerRegisteredEvent(string id, string displayName, string email) : base(entityId: id, entityType: DomainMetadata.Owner)
        {
            Id = id;
            DisplayName = displayName;
            Email = email;
        }


        public string Id { get; }
        public string DisplayName { get; }
        public string Email { get; }
    }
}
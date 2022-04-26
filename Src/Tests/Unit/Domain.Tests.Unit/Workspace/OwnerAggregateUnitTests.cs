﻿using FluentAssertions;
using MongoDB.Bson;
using NSubstitute;
using System;
using System.Linq;
using TaskoMask.Domain.Core.Exceptions;
using TaskoMask.Domain.Share.Helpers;
using TaskoMask.Domain.Share.Resources;
using TaskoMask.Domain.Tests.Unit.DataBuilders;
using TaskoMask.Domain.WriteModel.Workspace.Owners.Entities;
using TaskoMask.Domain.WriteModel.Workspace.Owners.Events.Owners;
using TaskoMask.Domain.WriteModel.Workspace.Owners.ValueObjects.Owners;
using Xunit;

namespace TaskoMask.Domain.Tests.Unit.Workspace
{
    public class OwnerAggregateUnitTests
    {
        public OwnerAggregateUnitTests()
        {

        }


        [Fact]
        public void Owner_Is_Constructed_Properly()
        {
            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail("Test@email.com")
                  .WithDisplayName("Test Name");

            //Act
            var owner = ownerBuilder.Build();


            //Assert
            owner.Id.Should().NotBeNullOrEmpty().And.Be(ownerBuilder.Id);
            owner.Email.Value.Should().NotBeNull().And.Be(ownerBuilder.Email);
            owner.DisplayName.Value.Should().NotBeNull().And.Be(ownerBuilder.DisplayName);
            owner.Organizations.Should().HaveCount(0);

        }



        [Fact]
        public void Owner_Created_Event_Is_Raised_When_Owner_Is_Constructed()
        {

            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail("Test@email.com")
                  .WithDisplayName("Test Name");

            var expectedEventType = nameof(OwnerCreatedEvent);

            //Act
            var owner = ownerBuilder.Build();

            //Assert
            owner.DomainEvents.Should().HaveCount(1);
            var domainEvent = owner.DomainEvents.First();
            domainEvent.EventType.Should().Be(expectedEventType);
            domainEvent.EntityId.Should().Be(owner.Id);

        }



        [Fact]
        public void Owner_Is_Not_Constructed_When_Id_Is_Null()
        {
            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(null)
                  .WithEmail("Test@email.com")
                  .WithDisplayName("Test Name");


            //Act
            Action act = () => ownerBuilder.Build();


            //Assert
            act.Should().Throw<DomainException>()
                .Where(e => e.Message.Equals(string.Format(DomainMessages.Null_Reference_Error, nameof(ownerBuilder.Id))));
        }



        [Fact]
        public void Owner_Is_Not_Constructed_When_DisplayName_Is_Null()
        {
            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail("Test@email.com")
                  .WithDisplayName(null);


            //Act
            Action act = () => ownerBuilder.Build();


            //Assert
            act.Should().Throw<DomainException>()
                .Where(e => e.Message.Equals(string.Format(DomainMessages.Required, nameof(OwnerDisplayName))));
        }



        [InlineData("H")]
        [InlineData("Ha")]
        [Theory]
        public void Owner_Is_Not_Constructed_When_DisplayName_Lenght_Is_Less_Than_Min_Length(string displayName)
        {
            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail("Test@email.com")
                  .WithDisplayName(displayName);


            //Act
            Action act = () => ownerBuilder.Build();


            //Assert
            act.Should().Throw<DomainException>()
                .Where(e => e.Message.Equals(string.Format(DomainMessages.Length_Error, nameof(OwnerDisplayName), DomainConstValues.Owner_DisplayName_Min_Length, DomainConstValues.Owner_DisplayName_Max_Length)));
        }


        [InlineData("Hamed@taskomask")]
        [InlineData("Ha.Taskomask.ir")]
        [InlineData("Ha.Taskomask")]
        [Theory]
        public void Owner_Is_Not_Constructed_When_Email_Is_Not_Valid(string email)
        {
            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail(email)
                  .WithDisplayName("Test Name");


            //Act
            Action act = () => ownerBuilder.Build();


            //Assert
            act.Should().Throw<DomainException>()
                .Where(e => e.Message.Equals(DomainMessages.Invalid_Email_Address));
        }


        [Fact]
        public void Owner_Updated_Event_Is_Raised_When_Owner_Is_Updated()
        {

            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail("Test@email.com")
                  .WithDisplayName("Test Name");

            var expectedEventType = nameof(OwnerUpdatedEvent);

            //Act
            var owner = ownerBuilder.Build();
            owner.UpdateOwner(
                OwnerDisplayName.Create("New Name"),
                OwnerEmail.Create("New@email.com"));

            //Assert
            owner.DomainEvents.Should().HaveCount(2);
            var domainEvent = owner.DomainEvents.Last();
            domainEvent.EventType.Should().Be(expectedEventType);
            domainEvent.EntityId.Should().Be(owner.Id);

        }


        [Fact]
        public void Organization_Is_Created_Properly()
        {

            //Arrange
            var ownerBuilder = OwnerBuilder.Init()
                  .WithId(ObjectId.GenerateNewId().ToString())
                  .WithEmail("Test@email.com")
                  .WithDisplayName("Test Name");


            //Act
            var owner = ownerBuilder.Build();
            var expectedOrganization = Organization.CreateOrganization("Test Organization Name", "Test Organization Description");
            owner.CreateOrganization(expectedOrganization);

            //Assert
            owner.Organizations.Should().HaveCount(1);
            var organization = owner.Organizations.First();
            organization.Name.Should().Be(expectedOrganization.Name);
            organization.Id.Should().Be(expectedOrganization.Id);
        }


    }
}

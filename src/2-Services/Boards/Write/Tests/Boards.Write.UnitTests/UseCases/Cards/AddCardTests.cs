﻿using FluentAssertions;
using MongoDB.Bson;
using NSubstitute;
using TaskoMask.BuildingBlocks.Contracts.Enums;
using TaskoMask.BuildingBlocks.Contracts.Events;
using TaskoMask.BuildingBlocks.Contracts.Resources;
using TaskoMask.BuildingBlocks.Domain.Resources;
using TaskoMask.Services.Boards.Write.Application.UseCases.Cards.AddCard;
using TaskoMask.Services.Boards.Write.Domain.Events.Cards;
using TaskoMask.Services.Boards.Write.UnitTests.Fixtures;
using Xunit;

namespace TaskoMask.Services.Cards.Write.UnitTests.UseCases.Cards
{
    public class AddCardTests : TestsBaseFixture
    {

        #region Fields

        private AddCardUseCase _addCardUseCase;

        #endregion

        #region Ctor

        public AddCardTests()
        {
        }

        #endregion

        #region Test Methods



        [Fact]
        public async Task Card_Is_Added()
        {
            //Arrange
            var expectedBoard = Boards.FirstOrDefault();
            var addCardRequest = new AddCardRequest(expectedBoard.Id, "Test_Name", BoardCardType.ToDo);

            //Act
            var result = await _addCardUseCase.Handle(addCardRequest, CancellationToken.None);

            //Assert
            result.Message.Should().Be(ContractsMessages.Create_Success);
            result.EntityId.Should().NotBeNull();

            var addedCard = expectedBoard.GetCardById(result.EntityId);
            addedCard.Should().NotBeNull();
            addedCard.Name.Value.Should().Be(addCardRequest.Name);
            addedCard.Type.Value.Should().Be(addCardRequest.Type);

            await InMemoryBus.Received(1).PublishEvent(Arg.Any<CardAddedEvent>());
            await MessageBus.Received(1).Publish(Arg.Any<CardAdded>());
        }



        [Fact]
        public async Task Adding_A_Card_Will_Throw_An_Exception_When_Board_Id_Is_Not_Existed()
        {
            //Arrange
            var notExistedBoard = ObjectId.GenerateNewId().ToString();
            var addCardRequest = new AddCardRequest(notExistedBoard, "Test_Name", BoardCardType.ToDo);
            var expectedMessage = string.Format(ContractsMessages.Data_Not_exist, DomainMetadata.Board);

            //Act
            Func<Task> act = async () => await _addCardUseCase.Handle(addCardRequest, CancellationToken.None);

            //Assert
            await act.Should().ThrowAsync<BuildingBlocks.Application.Exceptions.ApplicationException>().Where(e => e.Message.Equals(expectedMessage));
        }




        #endregion

        #region Fixture

        protected override void TestClassFixtureSetup()
        {
            _addCardUseCase = new AddCardUseCase(BoardAggregateRepository, MessageBus, InMemoryBus);
        }

        #endregion
    }
}

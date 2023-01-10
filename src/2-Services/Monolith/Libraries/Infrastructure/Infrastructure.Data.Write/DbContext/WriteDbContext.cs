﻿using MongoDB.Driver;
using TaskoMask.BuildingBlocks.Infrastructure.MongoDB;
using Microsoft.Extensions.Options;
using TaskoMask.Services.Monolith.Domain.DomainModel.Workspace.Boards.Entities;
using TaskoMask.Services.Monolith.Domain.DomainModel.Workspace.Tasks.Entities;

namespace TaskoMask.Services.Monolith.Infrastructure.Data.Write.DbContext
{

    /// <summary>
    /// 
    /// </summary>
    public class WriteDbContext : MongoDbContext, IWriteDbContext
    {
        #region Fields


        #endregion

        #region Ctors


        public WriteDbContext(IOptions<WriteDbOptions> mongoDbOptions) : base(mongoDbOptions)
        {
            Tasks = GetCollection<Task>();
        }



        #endregion

        #region Properties
        public IMongoCollection<Task> Tasks { get; }

        #endregion

    }
}

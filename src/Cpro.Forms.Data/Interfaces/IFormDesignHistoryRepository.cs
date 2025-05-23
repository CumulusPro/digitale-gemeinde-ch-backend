using Cpro.Forms.Data.Infrastructure;
using Cpro.Forms.Data.Models;
using Microsoft.EntityFrameworkCore;
using Peritos.Common.Abstractions;
using Peritos.Common.Data;

namespace Cpro.Forms.Data.Repositories;

public interface IFormDesignHistoryRepository : IRepository<FormDesignHistory>
{
    public Task<FormDesignHistory> SaveFormDesignHistoryAsync(FormDesignHistory formDesign);
    public Task<List<FormDesignHistory>> GetAllVersions(string formId);
    public Task<FormDesignHistory?> GetVersion(string formId, int version);
}
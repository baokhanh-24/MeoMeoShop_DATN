using MeoMeo.Domain.Commons;
using MeoMeo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.Domain.IRepositories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        // All CRUD operations are inherited from IBaseRepository
        // Custom methods can be added here if needed in the future
    }
} 
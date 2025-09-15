using MeoMeo.Domain.Entities;
using MeoMeo.Domain.IRepositories;
using MeoMeo.EntityFrameworkCore.Commons;
using MeoMeo.EntityFrameworkCore.Configurations.Contexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeoMeo.EntityFrameworkCore.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(MeoMeoDbContext context) : base(context)
        {
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            var employeeAdded = await AddAsync(employee);
            return employeeAdded;
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            await DeleteAsync(id);
            return true;
        }

        public async Task<List<Employee>> GetAllEmployeeAsync()
        {
            var getAllEmployee = await GetAllAsync();
            return getAllEmployee.ToList();
        }

        public async Task<Employee> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _context.employees
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == id);
            return employee;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            var employeeUpdated = await UpdateAsync(employee);
            return employeeUpdated;
        }
    }
}

using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Services
{
    public interface ITaskListService
    {
        public Task<IEnumerable<TaskList>> GetAllAsync(HttpClient httpClient);
        public Task<IEnumerable<TaskList>> GetAllAsync(HttpClient httpClient, string filter);
        public Task<TaskList> PatchObjectAsync(HttpClient httpClient, TaskList taskList, Guid oid);
    }
}
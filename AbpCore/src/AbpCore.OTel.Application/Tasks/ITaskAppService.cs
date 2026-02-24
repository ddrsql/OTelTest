using AbpCore.OTel.Tasks.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpCore.OTel.Tasks
{
    public interface ITaskAppService
    {
        GetTasksOutput GetTasks(GetTasksInput input);
        void UpdateTask(UpdateTaskInput input);

        void CreateTask(CreateTaskInput input);
    }
}

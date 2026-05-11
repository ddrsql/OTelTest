using Abp.Application.Services;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Repositories;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AbpFramework.OTel.Authorization;
using AbpFramework.OTel.Authorization.Users;
using AbpFramework.OTel.Tasks.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbpFramework.OTel.Tasks
{
    //[AbpAuthorize(PermissionNames.Pages_Tasks)]
    public class TaskAppService : OTelAppServiceBase, ITaskAppService, ITransientDependency
    {
        //These members set in constructor using constructor injection.

        private readonly IRepository<Task,long> _taskRepository;
        private readonly IRepository<User,long> _userRepository;

        /// <summary>
        ///In constructor, we can get needed classes/interfaces.
        ///They are sent here by dependency injection system automatically.
        /// </summary>
        public TaskAppService(IRepository<Task, long> taskRepository, IRepository<User, long> userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public virtual GetTasksOutput GetTasks(GetTasksInput input)
        {
            Test();
            var tasks = _taskRepository.GetAllIncluding(x => x.AssignedUser)
                .WhereIf(input.State != 0, x => x.State == input.State);

            //Used AutoMapper to automatically convert List<Task> to List<TaskDto>.
            return new GetTasksOutput
            {
                Tasks = ObjectMapper.Map<List<TaskDto>>(tasks)
            };
        }


        public virtual void Test()
        {
            Logger.Info($"{nameof(TaskAppService)}.{nameof(Test)}");
            TestPublic();
            TestPublicVirtual();
            TestPrivate();
        }
        public virtual void TestPublic()
        {
            Logger.Info($"{nameof(TaskAppService)}.{nameof(TestPublic)}");
        }
        public virtual void TestPublicVirtual()
        {
            Logger.Info($"{nameof(TaskAppService)}.{nameof(TestPublicVirtual)}");
        }
        private void TestPrivate()
        {
            Logger.Info($"{nameof(TaskAppService)}.{nameof(TestPrivate)}");
        }

        public virtual void UpdateTask(UpdateTaskInput input)
        {

            //Retrieving a task entity with given id using standard Get method of repositories.
            var task = _taskRepository.Get(input.Id);

            //Updating changed properties of the retrieved task entity.

            if (input.State.HasValue)
            {
                task.State = input.State.Value;
            }

            if (AbpSession.UserId.HasValue)
            {
                task.AssignedUser = _userRepository.Load(AbpSession.UserId.Value);
            }

            //We even do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).
        }

        public virtual void CreateTask(CreateTaskInput input)
        {
            //Creating a new Task entity with given input's properties
            var task = ObjectMapper.Map<Task>(input);
            

            if (AbpSession.UserId.HasValue)
            {
                task.AssignedUserId = input.AssignedUserId;
            }

            //Saving entity with standard Insert method of repositories.
            _taskRepository.Insert(task);
        }
    }
}

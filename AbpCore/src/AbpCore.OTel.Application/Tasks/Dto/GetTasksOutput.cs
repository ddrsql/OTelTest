using System.Collections.Generic;

namespace AbpCore.OTel.Tasks.Dto
{
    public class GetTasksOutput
    {
        public List<TaskDto> Tasks { get; set; }
    }
}

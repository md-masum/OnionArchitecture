using Core.Entity;
using Core.Interfaces.Common;

namespace Core.Dto
{
    public class TestDto : BaseDto, IMapFrom<Test>
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}

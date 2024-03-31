using Keyed.Abstractions;

namespace Keyed.Tests.Services
{
    public class AssignmentHandler : IKeyedObject<AssignmentType>
    {
        public AssignmentType Key { get; }

        public AssignmentHandler(AssignmentType key)
        {
            Key = key;
        }
    }
}

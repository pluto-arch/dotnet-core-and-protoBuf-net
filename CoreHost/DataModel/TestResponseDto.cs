using ProtoBuf;

namespace DataModel
{
    [ProtoContract]
    public class TestResponseDto:IBaseDto
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string FirstName { get; set; }
        [ProtoMember(3)]
        public string LastName { get; set; }
    }
}
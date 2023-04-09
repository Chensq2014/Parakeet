using System;

namespace Parakeet.Net.Dtos
{
    public class IdNameDto<TId, TName>
    {
        public IdNameDto()
        {

        }

        public IdNameDto(TId id, TName name)
        {
            Id = id;
            Name = name;
        }
        public TId Id { get; set; }
        public TName Name { get; set; }

    }

    public class IdNameDto : IdNameDto<Guid, string>
    {
        public IdNameDto()
        {
        }

        public IdNameDto(Guid value, string text)
            : base(value, text)
        {

        }
    }
}

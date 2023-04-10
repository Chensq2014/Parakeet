using System;
using System.Collections.Generic;

namespace Parakeet.Net.Dtos
{
    public class SelectBox<TText, TValue>
    {
        public SelectBox()
        {
        }

        public SelectBox(TText text, TValue value)
        {
            Text = text;
            Value = value;
        }
        public TText Text { get; set; }
        public TValue Value { get; set; }
        public List<SelectBox<TText, TValue>> Children { get; set; }
    }

    public class SelectBox : SelectBox<string, Guid>
    {
        public SelectBox()
        {
        }

        public SelectBox(string text, Guid value)
            : base(text, value)
        {
        }
    }
}

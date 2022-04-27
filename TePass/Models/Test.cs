using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using TePass.ViewModels;

namespace TePass.Models
{
    public class Test
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public List<Question> Questions { get; set; }
        public List<Result> Results { get; set; }
        public int UserId { get; set; }
        public override bool Equals(object obj)
        {
            Test test = obj as Test;
            return this.Id == test.Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

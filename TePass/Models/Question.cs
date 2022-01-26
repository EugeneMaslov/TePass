using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using TePass.ViewModels;

namespace TePass.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string OQuestion { get; set; }
        public List<Varient> Varients { get; set; }
        public int TestId { get; set; }
        public override bool Equals(object obj)
        {
            Question question = obj as Question;
            return this.Id == question.Id;
        }
    }
}

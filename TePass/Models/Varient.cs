using System;
using System.Collections.Generic;
using System.Text;

namespace TePass.Models
{
    public class Varient : ICloneable
    {
        public int Id { get; set; }
        public string OVarient { get; set; }
        public bool IsTrue { get; set; }
        public int QuestionId { get; set; }

        public object Clone()
        {
            return new Varient(Id,OVarient, IsTrue, QuestionId);
        }
        public object CloneNotTrue()
        {
            return new Varient(Id, OVarient, false, QuestionId);
        }
        public Varient()
        {
            
        }
        public Varient(int id, string ovarient, bool istrue, int questid)
        {
            Id = id;
            OVarient = ovarient;
            IsTrue = istrue;
            QuestionId = questid;
        }

        public override bool Equals(object obj)
        {
            Varient varient = obj as Varient;
            return this.Id == varient.Id;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

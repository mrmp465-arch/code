using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace APIMobiNext.Entity
{
    public class Roulette
    {
        public bool display { get; set; }
    }

    public class UserStage
    {
        public string name { get; set; }
        public string level { get; set; }
        public string logoUrl { get; set; }
        public int currentPoint { get; set; }
        public string nextStageName { get; set; }
        public int nextStagePoint { get; set; }
        public int needPointNextStage { get; set; }
    }

    public class ResultAddPoint
    {
        public int pointPlus { get; set; }
        public string newStageName { get; set; }
        public bool isNextStage { get; set; }
        public bool isHasTicket { get; set; }
        public UserStage userStage { get; set; }
        public List<object> ticket { get; set; }
    }

    public class TopupResponse
    {
        public bool isSuccess { get; set; }
        public int valueTopupSuccess { get; set; }
        public Roulette roulette { get; set; }
        public ResultAddPoint resultAddPoint { get; set; }
    }

    public class TopupErrorResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public string fields { get; set; }
    }

    public class TopupSuccessResponse
    {
        public string result { get; set; }
    }
}
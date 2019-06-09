using System;
using System.Collections.Generic;
using System.Text;

namespace RestApiTest.Core.Interfaces
{
    public interface IVotable
    {
        IVotable GetVotableObject();
        void RemoveReferenceToVote(long id);
    }
}

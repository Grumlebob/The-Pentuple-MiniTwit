using System;
using System.Collections.Generic;

namespace MiniTwit.Api.Domain;

public partial class Follower
{
    public int? WhoId { get; set; }

    public int? WhomId { get; set; }
}

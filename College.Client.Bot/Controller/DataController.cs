using System;

namespace College.Client.Bot.Controller;

public class ISubGroup {
    public required string SClID {get; set;}
    public required string SGrID {get; set;}
    public required string SGCaID {get; set;}
    public required string STopic {get; set;}
    public required string STitle {get; set;}
}

public class IApiResponse {
    public required string ClID {get; set;}
    public required string Day {get; set;}
    public required string group {get; set;}
    public required string topic {get; set;}
    public required string start {get; set;}
    public required string end {get; set;}
    public required string room {get; set;}
    public required string color {get; set;}
    public required string title {get; set;}
    public List<ISubGroup> SubGroup {get; set;}
}

class PostData {
    public required string d_start {get; set;}
    public required string d_end {get; set;}
    public required string group {get; set;}
}
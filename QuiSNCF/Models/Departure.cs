namespace QuiSNCF.Models;

public record Departure(
    string ScheduledTime,   
    string Destination,     
    string Train,           
    string Mode,          
    int DelayMinutes       
);
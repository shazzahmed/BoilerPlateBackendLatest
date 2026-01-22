using Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class AlumniEvent : BaseEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    [MaxLength(100)]
    public string EventFor { get; set; }
    [Required]
    public int SessionId { get; set; }
    [Required]
    public string ClassId { get; set; }
    public string Section { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Note { get; set; }
    [MaxLength(255)]
    public string Photo { get; set; }
    public string EventNotificationMessage { get; set; }
    public bool ShowOnWebsite { get; set; }

}

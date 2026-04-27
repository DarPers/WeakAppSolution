using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessorService.DAL.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

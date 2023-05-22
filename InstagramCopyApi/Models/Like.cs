using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace InstagramCopyApi.Models;

public partial class Like
{
    [Key]
    public int Id { get; set; }

    public int? PostId { get; set; }

    public int? CommentId { get; set; }

    public int? UserId { get; set; }

}

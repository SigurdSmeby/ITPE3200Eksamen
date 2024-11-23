using System.ComponentModel.DataAnnotations.Schema;

namespace server.Models
{
    public class Follow
    {
        [ForeignKey("Followers")]
        public int FollowerId { get; set; }
        public virtual User Follower { get; set; }

        [ForeignKey("Following")]
        public int FollowingId { get; set; }
        public virtual User Following { get; set; }
    }
}

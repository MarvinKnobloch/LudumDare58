using UnityEngine;

public class PlayerCollision
{
    public Player player;

    private float groundCheckRange = 0.05f;

    public void GroundCheck()
    {
        RaycastHit2D downwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.99f, 0, -player.transform.up, groundCheckRange, player.groundCheckLayer);
        if (downwardhit)
        {
            Debug.DrawRay(downwardhit.point, downwardhit.normal, Color.green);
        }
        else
        {
            player.SwitchGroundIntoAir();
        }
    }
    public void AirCheck()
    {
        float velocity = player.rb.linearVelocity.y;

        //if (player.movingPlatform != null)
        //{
        //    velocity -= player.movingPlatform.velocity.y;
        //}

        if (velocity <= 0.1f)
        {
            ForwardCheck();

            RaycastHit2D downwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.99f, 0, -player.transform.up, groundCheckRange, player.groundCheckLayer);
            if (downwardhit)
            {
                {
                    player.SwitchToGround(false);
                }
            }
        }
        else
        {
            ForwardCheck();
        }
    }
    private void ForwardCheck()
    {
        if (!player.faceRight)
        {
            RaycastHit2D forwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.96f, 0, player.transform.right, 0.2f, player.groundCheckLayer);
            if (forwardhit)
            {
                player.playerVelocity.Set(0, player.rb.linearVelocity.y);
                player.rb.linearVelocity = player.playerVelocity;
            }
        }
        else
        {
            RaycastHit2D forwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.96f, 0, -player.transform.right, 0.2f, player.groundCheckLayer);

            if (forwardhit)
            {
                player.playerVelocity.Set(0, player.rb.linearVelocity.y);
                player.rb.linearVelocity = player.playerVelocity;
            }
        }
    }
    public void CollisionCheckAfterAbilties()
    {
        RaycastHit2D downwardhit = Physics2D.BoxCast(player.playerCollider.bounds.center, player.playerCollider.bounds.size * 0.99f, 0, -player.transform.up, groundCheckRange, player.groundCheckLayer);
        if (downwardhit)
        {
            player.SwitchToGround(false);
        }
        else
        {
            player.SwitchToAir();
        }
    }
}

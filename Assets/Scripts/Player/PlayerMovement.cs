using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement
{
    public Player player;
    private float dashTimer;

    const string idleState = "Idle";
    const string runState = "Run";
    const string jumpState = "Jump";
    const string fallState = "Fall";
    const string dashState = "Dash";

    //private float platformGroundDrag = -10;

    public void PlayerMove(float grounddrag)
    {
        player.playerVelocity.Set(player.moveDirection.x * player.movementSpeed, grounddrag);

        player.rb.linearVelocity = player.playerVelocity;

        //Animation
        if (player.state == Player.States.Ground)
        {
            if (player.moveDirection == Vector2.zero)
            {
                player.ChangeAnimationState(idleState);
            }
            else
            {
                player.ChangeAnimationState(runState);
            }
        }
    }
    public void GroundMovement()
    {
        PlayerMove(player.playerGroundDrag);
    }
    public void GroundIntoAirTransition()
    {
        player.groundIntoAirTimer += Time.deltaTime;

        if (player.groundIntoAirTimer > player.groundIntoAirTransitionTime)
        {
            player.SwitchToAir();
        }
    }
    public void AirMovement()
    {
        if (player.rb.linearVelocity.y < player.maxFallSpeed) PlayerMove(player.maxFallSpeed);
        else PlayerMove(player.rb.linearVelocity.y);

        if (player.rb.linearVelocity.y < 2)
        {
            player.ChangeAnimationState(fallState);
        }
        else
        {
            player.ChangeAnimationState(jumpState);
        }
    }
    public void RotatePlayer()
    {
        if (player.moveDirection.x > 0 && player.faceRight == true) flip();
        if (player.moveDirection.x < 0 && player.faceRight == false) flip();
    }
    private void flip()
    {
        player.faceRight = !player.faceRight;
        Vector3 localScale;
        localScale = player.transform.localScale;
        localScale.x *= -1;
        player.transform.localScale = localScale;
    }
    public void JumpInput(InputAction.CallbackContext ctx)
    {

        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            JumpInputPerformed();
        }
    }
    private void JumpInputPerformed()
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;

        int count = player.maxJumpCount;
        if (player.currentJumpCount >= count) return;

        switch (player.state)
        {
            case Player.States.Ground:
                Jump();
                break;
            case Player.States.GroundIntoAir:
                Jump();
                break;
            case Player.States.Air:
                Jump();
                break;
        }
    }
    private void Jump()
    {
        player.currentJumpCount++;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.AddForce(new Vector2(0, player.jumpStrength), ForceMode2D.Impulse);

        player.jumpPerformed = true;
        player.jumpTimer = 0;
        //player.ChangeAnimationState(jumpState);

        if (player.state != Player.States.Air) player.SwitchGroundIntoAir();

    }
    public void JumpIsPressed()
    {
        if (player.jumpPerformed == false) return;

        player.jumpTimer += Time.deltaTime;
        if (player.jumpTimer > player.maxJumpTime)
        {
            player.jumpPerformed = false;
        }
        if (player.controls.Player.Jump.WasReleasedThisFrame())
        {
            float velocityReduce = player.maxJumpTime - player.jumpTimer;
            player.rb.AddForce(new Vector2(0, velocityReduce * -player.jumpCancelVelocityMultiplier), ForceMode2D.Impulse);
            player.jumpPerformed = false;
        }
    }
    public void DashInput(InputAction.CallbackContext ctx)
    {
        bool pressed = ctx.ReadValueAsButton();
        if (pressed)
        {
            DashInputPerformed();
        }
    }
    private void DashInputPerformed()
    {
        if (IngameController.Instance.menuController.gameIsPaused) return;
        if (player.currentDashCount >= player.maxDashCount) return;

        switch (player.state)
        {
            case Player.States.Ground:
                StartDash();
                break;
            case Player.States.GroundIntoAir:
                StartDash();
                break;
            case Player.States.Air:
                StartDash();
                break;
            case Player.States.Attack:
                StartDash();
                break;
        }
    }
    private void StartDash()
    {
        player.currentDashCount++;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;

        RotatePlayer();

        player.ChangeAnimationState(dashState);
        dashTimer = 0;
        player.state = Player.States.Dash;
    }
    public void DashMovement()
    {
        Vector2 movement = new Vector2(player.dashStrength, 0);

        if (player.faceRight) player.rb.linearVelocity = movement * -player.transform.right;
        else player.rb.linearVelocity = movement * player.transform.right;
    }
    public void DashTime()
    {
        dashTimer += Time.deltaTime;
        if (dashTimer >= player.dashTime)
        {
            player.SwitchToAir();
        }
    }
}

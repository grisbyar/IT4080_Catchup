using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public float movementSpeed = 50f;
    public float rotationSpeed = 130f;
    public NetworkVariable<Color> playerColorNetVar = new NetworkVariable<Color>(Color.red);

    private Camera playerCamera;
    private GameObject playerLine; //playerBody

    private void NetworkInit(){
        playerCamera = transform.Find("Camera").GetComponent<Camera>();
        playerCamera.enabled = IsOwner;
        playerCamera.GetComponent<AudioListener>().enabled = IsOwner;
    
        playerLine = transform.Find("Line").gameObject; //Findplayerbody
        ApplyColor();
    }

    private void Start(){
       NetworkHelper.Log(this, "Start");
    }

    public override void OnNetworkSpawn(){
        NetworkHelper.Log(this, "OnNetworkSpawn");
        NetworkInit();
        base.OnNetworkSpawn();
    }

   
    // Rotate around the y axis when shift is not pressed
    private Vector3 CalcRotation() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        Vector3 rotVect = Vector3.zero;
        if (!isShiftKeyDown) {
            rotVect = new Vector3(0, Input.GetAxis("Horizontal"), 0);
            rotVect *= rotationSpeed * Time.deltaTime;
        }
        return rotVect;
    }


    // Move up and back, and strafe when shift is pressed
    private Vector3 CalcMovement() {
        bool isShiftKeyDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        float x_move = 0.0f;
        float z_move = Input.GetAxis("Vertical");

        if (isShiftKeyDown) {
            x_move = Input.GetAxis("Horizontal");
        }

        Vector3 moveVect = new Vector3(x_move, 0, z_move);
        moveVect *= movementSpeed * Time.deltaTime;

        return moveVect;
    }

    private void OwnerHandleInput()
    {
        Vector3 movement = CalcMovement();
        Vector3 rotation = CalcRotation();
        if(movement != Vector3.zero || rotation != Vector3.zero){
            MoveServerRpc(CalcMovement(), CalcRotation());
        }
        
    }
    private void ApplyColor()
    {
        playerLine.GetComponent<MeshRenderer>().material.color = playerColorNetVar.Value;
    }

    [ServerRpc]
    private void MoveServerRpc(Vector3 movement, Vector3 rotation){
        transform.Translate(movement);
        transform.Rotate(rotation);
    }
    // private Vector3 getPlayerPos(){
        
    // }

    private bool outOfBounds()
    {
        Vector3 position = GetComponent<PlayerCharacterOne>().position;
   
        if (position.x > 10 || position.z > 10 || position.x < -10 || position.z < -10 ){
            //don't need to check y axis
            return true;
        }
        return false;
    }
    private void returnToStartPos(){
        player.position = Vector3.zero;
    }

    private void Update()
    {
        if(IsOwner){
            OwnerHandleInput();
        }
        //checks if anyone but the owner is out of bounds
        if(!IsOwner && outOfBounds){
            returnToStartPos();
        }
    }
}

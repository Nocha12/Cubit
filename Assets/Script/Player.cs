using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public class Player : MonoBehaviour
{
    public enum Skin
    {
        Normal,
        Angel,
        Devil
    }

    [Serializable]
    public class SkinC
    {
        public Skin skin;
        public Material inMaterial;
        public Material outMaterial;
        public GameObject[] accessories;
    }
    public Renderer[] renderers;

    public Joystick joystick;
    public Animator ani;
    public Coll groundCheckCol;
    public Coll frontCheckCol;
    public Transform CamT;
    public Transform game;

    public List<SkinC> skins;
    private SkinC beforeSkin;

    private Rigidbody rig;
    private RaycastHit[] beforeHits;

    public void ChangeSkin(Skin skinE)
    {
        if (beforeSkin != null)
            foreach (var obj in beforeSkin.accessories)
                obj.SetActive(false);

        var s = skins.Find(q=>q.skin == skinE);

        foreach (var obj in s.accessories)
            obj.SetActive(true);

        foreach (var r in renderers)
        {
            if (r.materials.Length == 1)
                r.material = s.outMaterial;
            else if (r.materials.Length == 2)
            {
                var m = r.materials;
                m[0] = s.outMaterial;
                m[1] = s.inMaterial;
                r.materials = m;
            }
            else
            {
                var m = r.materials;
                m[0] = s.outMaterial;
                m[2] = s.inMaterial;
                r.materials = m;
            }
        }
        
        beforeSkin = s;
    }

    public void Reset()
    {
        rig = GetComponent<Rigidbody>();

        ani.speed = 1;
        ani.SetBool("isWalk", false);
        ani.SetBool("isJump", false);
        transform.position = new Vector3(0, 0.5f, 0);
        transform.localRotation = Quaternion.Euler(0, 135, 0);

        rig.constraints = RigidbodyConstraints.FreezeRotation;

        joystick.handle.transform.position = joystick.transform.position;
        joystick.result = new Vector3(0, 0, 0);

        groundCheckCol.isCol = true;
        frontCheckCol.isCol = false;
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (GameManager.instance.state != GameManager.State.Play)
            return;

        JoystickMoving();

        if((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 1 && Input.GetTouch(1).phase == TouchPhase.Began && !GameManager.IsPointerOverUIObject(Input.GetTouch(1).position))) && groundCheckCol.isCol && rig.velocity.y <= 0)
        {
            if (GameManager.instance.tutorialPhase == 1 && GameManager.instance.continueTutorial)
            {
                GameManager.instance.tutorialPhase = 2;
                GameManager.instance.continueTutorial = false;
            }

            rig.velocity = new Vector3(0, 4, 0);
            ani.SetBool("isJump", true);
        }
        if(groundCheckCol.isCol && rig.velocity.y <= 0)
            ani.SetBool("isJump", false);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position + new Vector3(0, 0.07f), CamT.position - transform.position, Vector3.Distance(CamT.position, transform.position));if (beforeHits != null)
            foreach (var obj in beforeHits)
                if(!hits.Contains(obj))
                    obj.transform.GetComponent<Box>().isDisabled = false;

        foreach (var obj in hits)
        {
            if (!obj.transform.CompareTag("Cube"))
                return;

            if (obj.transform.GetComponent<Box>().isFalling)
                continue;

            obj.transform.GetComponent<Box>().isDisabled = true;

            foreach (var renderer in obj.transform.GetComponentsInChildren<MeshRenderer>())
            {
                StandardShaderUtils.ChangeRenderMode(renderer.material, StandardShaderUtils.BlendMode.Transparent);
                renderer.material.DisableKeyword("_EMISSION");
                Color c = renderer.material.color;
                c.a -= 0.1f;
                if (c.a < 0.1f)
                    c.a = 0.1f;
                renderer.material.color = c;
            }
        }

        beforeHits = hits;
    }

    void JoystickMoving()
    {
        if (joystick.result.magnitude < 0.1f)
        {
            ani.SetBool("isWalk", false);
            return;
        }

        ani.SetBool("isWalk", true);

        float angle = -Mathf.Atan2(joystick.result.y, joystick.result.x);

        transform.eulerAngles = new Vector3(0, angle * Mathf.Rad2Deg - 135 + game.transform.eulerAngles.y, 0);

        if (frontCheckCol.isCol)
            return;

        Vector3 moveVec = transform.forward * joystick.result.magnitude * 0.022f * Time.deltaTime;
        Vector3 p = transform.position;

        if (moveVec.x + p.x < -0.55f || moveVec.x + p.x > 0.55f)
            moveVec.x = 0;
        if (moveVec.z + p.z < -0.55f || moveVec.z + p.z > 0.55f)
            moveVec.z = 0;

        transform.position += new Vector3(moveVec.x, 0, moveVec.z);

        moveVec.x = Mathf.Clamp(transform.position.x, -0.55f, 0.55f);
        moveVec.z = Mathf.Clamp(transform.position.z, -0.55f, 0.55f);

        transform.position = new Vector3(moveVec.x, transform.position.y, moveVec.z);
    }
}

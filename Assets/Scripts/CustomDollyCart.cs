using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomDollyCart : MonoBehaviour
{
    /// <summary>The path to follow</summary>
    [Tooltip("The path to follow")]
    public CinemachinePathBase m_Path;

    /// <summary>How to interpret the Path Position</summary>
    [Tooltip("How to interpret the Path Position.  If set to Path Units, values are as follows: 0 represents the first waypoint on the path, 1 is the second, and so on.  Values in-between are points on the path in between the waypoints.  If set to Distance, then Path Position represents distance along the path.")]
    public CinemachinePathBase.PositionUnits m_PositionUnits = CinemachinePathBase.PositionUnits.Normalized;

    /// <summary>Move the cart with this speed</summary>
    [Tooltip("Move the cart with this speed along the path.  The value is interpreted according to the Position Units setting.")]
    public float m_Speed;

    public List<CustomCart> cartTransforms;

    private void Start()
    {
        cartTransforms = new List<CustomCart>();
    }


    void Update()
    {
        float speed = Application.isPlaying ? m_Speed : 0;

        if (cartTransforms.Count > 0)
        {
            try
            {
                for (int i = cartTransforms.Count - 1; i <= 0; i--)
                {
                    if (cartTransforms[i].cartPosition >= 0.95f)
                    {
                        cartTransforms[i].cartTransform.gameObject.SetActive(false);
                        cartTransforms.RemoveAt(i);
                        //break to prevent list iteration during list editing
                        break;
                    }
                }
            }
            catch (Exception e) { }
        }
        for (int i = 0; i < cartTransforms.Count; i++)
        {
            SetCartPosition(i, cartTransforms[i].cartPosition + speed * Time.deltaTime);
        }
    }


    void SetCartPosition(int cart, float distanceAlongPath)
    {
        if (m_Path != null)
        {
            cartTransforms[cart].cartPosition = m_Path.StandardizeUnit(distanceAlongPath, m_PositionUnits);
            cartTransforms[cart].cartTransform.position = m_Path.EvaluatePositionAtUnit(cartTransforms[cart].cartPosition, m_PositionUnits);
            cartTransforms[cart].cartTransform.rotation = m_Path.EvaluateOrientationAtUnit(cartTransforms[cart].cartPosition, m_PositionUnits);
        }
    }

    public void AddCart(Transform transform)
    {
        CustomCart cart = new CustomCart();
        cart.cartTransform = transform;
        cart.cartPosition = 0;
        if(cartTransforms != null)
            cartTransforms.Add(cart);
    }

    public class CustomCart
    {
        public Transform cartTransform;
        public float cartPosition;
    }
}

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Vuforia;

public class TrackingHandler : MonoBehaviour
{
    public string ProductID;
    public GameObject Model;
    public ListGenerationBehaviour Subscriber;

    private TrackableBehaviour mTrackableBehaviour;
    private TrackableBehaviour.Status m_PreviousStatus;
    private TrackableBehaviour.Status m_NewStatus;
    protected bool m_CallbackReceivedOnce = false;
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour) {
            mTrackableBehaviour.RegisterOnTrackableStatusChanged(OnTrackableStatusChanged);
        }   
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTrackableStatusChanged(TrackableBehaviour.StatusChangeResult statusChangeResult) {
        m_PreviousStatus = statusChangeResult.PreviousStatus;
        m_NewStatus = statusChangeResult.NewStatus;

        HandleTrackableStatusChanged();
    }
    
    protected virtual void HandleTrackableStatusChanged()
    {
        if (!ShouldBeRendered(m_PreviousStatus) &&
            ShouldBeRendered(m_NewStatus))
        {
            OnTrackingFound();
        }
        else if (ShouldBeRendered(m_PreviousStatus) &&
                 !ShouldBeRendered(m_NewStatus))
        {
            OnTrackingLost();
        }
        else
        {
            if (!m_CallbackReceivedOnce && !ShouldBeRendered(m_NewStatus))
            {
                // This is the first time we are receiving this callback, and the target is not visible yet.
                // --> Hide the augmentation.
                OnTrackingLost();
            }
        }

        m_CallbackReceivedOnce = true;
    }

    protected bool ShouldBeRendered(TrackableBehaviour.Status status)
    {
        if (status == TrackableBehaviour.Status.DETECTED ||
            status == TrackableBehaviour.Status.TRACKED)
        {
            return true;
        }

        return false;
    }

    protected virtual void OnTrackingFound() {
        if (mTrackableBehaviour && Model) {
            Model.SetActive(true);
            PlaceModelOnImageTarget();
            Subscriber.notify(this, true);
        }
    }
    protected virtual void OnTrackingLost() {
        if (mTrackableBehaviour && Model) {
            Subscriber.notify(this, false);
            Model.SetActive(false);
        }
    }

    private void PlaceModelOnImageTarget() {
            Model.transform.parent = mTrackableBehaviour.transform; // Attach Model to trackable
            Model.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            Model.transform.localPosition = new Vector3(0.0f, 0.05f, 0.0f);
            Model.transform.localRotation = Quaternion.identity;
    }
}

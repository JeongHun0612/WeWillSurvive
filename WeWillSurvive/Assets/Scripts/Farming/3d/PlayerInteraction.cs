using UnityEngine;

namespace WeWillSurvive
{
    public class PlayerInteraction : MonoBehaviour
    {
        public GameObject firstPersonCam;
        public GameObject thirdPersonCam;
        public Camera playercamera;

        public LayerMask interactableLayerMask; // Interactable 레이어 지정
        public float interactRange = 2f; // 상호작용 최대 거리
        public float interactRadius = 0.5f;
        private float holdTimer = 0f;

        private bool isFirstPerson = true;

        private bool canInteract = false;
        private Interactible currentInteractible = null;

        void Update()
        {
            // 카메라 전환
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                isFirstPerson = !isFirstPerson;
                firstPersonCam.SetActive(isFirstPerson);
                thirdPersonCam.SetActive(!isFirstPerson);

                Cursor.lockState = isFirstPerson ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = !isFirstPerson;

                // 기존 상호작용 UI 숨기기
                if (currentInteractible != null)
                {
                    currentInteractible.HideInteractionUI();
                }
                currentInteractible = null;
                canInteract = false;
            }

            DetectInteractible();

            if (canInteract && currentInteractible != null)
            {

                if (Input.GetKey(KeyCode.Space) && currentInteractible.interactibleType == InteractibleType.Putout)
                {
                    holdTimer += Time.deltaTime;
                    if (holdTimer >= 0.4f)
                    {
                        if (currentInteractible != null) { currentInteractible.Escape(); }
                    }
                }
                else
                {
                    holdTimer = 0f;
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (currentInteractible != null) { currentInteractible?.Interact(); }
                }
            }
        }

        void DetectInteractible()
        {
            // 이전 interactible가 있으면 UI 숨김 처리
            if (currentInteractible != null)
            {
                currentInteractible.HideInteractionUI();
                currentInteractible = null;
                canInteract = false;
            }

            if (isFirstPerson)
            {
                Ray ray = playercamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
                Vector3 origin = ray.origin - ray.direction * interactRadius;
                if (Physics.SphereCast(origin, interactRadius, ray.direction, out RaycastHit hit, interactRange, interactableLayerMask))
                {
                    var interactible = hit.collider.GetComponent<Interactible>();
                    if (interactible != null)
                    {
                        currentInteractible = interactible;
                        canInteract = true;
                        currentInteractible.ShowInteractionUI();
                        return;
                    }
                }
            }
            else
            {
                Collider[] hits = Physics.OverlapSphere(transform.position, interactRange, interactableLayerMask);
                Interactible closest = null;
                float minDistance = Mathf.Infinity;

                foreach (var col in hits)
                {
                    Vector3 viewportPos = playercamera.WorldToViewportPoint(col.transform.position);
                    if (viewportPos.z > 0 &&
                        viewportPos.x >= 0 && viewportPos.x <= 1 &&
                        viewportPos.y >= 0 && viewportPos.y <= 1)
                    {
                        float dist = Vector3.Distance(transform.position, col.transform.position);
                        if (dist < minDistance)
                        {
                            var interactible = col.GetComponent<Interactible>();
                            if (interactible != null)
                            {
                                minDistance = dist;
                                closest = interactible;
                            }
                        }
                    }
                }

                if (closest != null)
                {
                    currentInteractible = closest;
                    canInteract = true;
                    if (currentInteractible != null) { currentInteractible.ShowInteractionUI(); }
                    return;
                }
            }

            // 상호작용 대상 없으면 UI 해제 (이미 처리됨)
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG
{
    public class UIElementDragger : MonoBehaviour
    {

        public const string DRAGGABLE_TAG = "UIDraggable";

        private bool dragging = false;

        private Vector2 originalPosition;
        private Transform originalParent;

        private IDraggable objectToDrag;
        private Image objectToDragImage;

        List<RaycastResult> hitObjects = new List<RaycastResult>();

        void Update()
        {
            // on mouse down, start dragging if we find a draggable
            if (Input.GetMouseButtonDown(0))
            {
                objectToDrag = GetDraggableTransformUnderMouse();

                // check with the container to see if we can drag objects
                if (objectToDrag != null)
                {
                    IDragAndDropContainer containerDrag = objectToDrag.GetContainer();
                    if (containerDrag != null && containerDrag.CanDrag(objectToDrag) == false)
                        objectToDrag = null;
                }

                if (objectToDrag != null)
                {
                    dragging = true;

                    // do this so the dragged object appears on top of other sibling containers
                    //objectToDrag.transform.parent.SetAsLastSibling();

                    originalPosition = objectToDrag.transform.position;
                    originalParent = objectToDrag.transform.parent;

                    Transform parentCanvas = objectToDrag.transform.parent;
                    while (parentCanvas != null && parentCanvas.GetComponent<Canvas>() == null)
                        parentCanvas = parentCanvas.parent;
                    objectToDrag.transform.parent = parentCanvas;
                    parentCanvas.SetAsLastSibling();

                    // make the dragged object appear over all other objects in the container
                    objectToDrag.transform.SetAsLastSibling();

                    // make the object being dragged transparent to raycasts
                    objectToDragImage = objectToDrag.GetComponent<Image>();
                    objectToDragImage.raycastTarget = false;
                }
            }

            if (dragging)
                objectToDrag.transform.position = Input.mousePosition;

            // on mouse up, try to complete the drag and drop
            if (Input.GetMouseButtonUp(0))
            {
                if (objectToDrag != null)
                {
                    IDraggable objectToReplace = GetDraggableTransformUnderMouse();

                    // find the container we're dropping in to and from
                    IDragAndDropContainer containerReplace = null;
                    IDragAndDropContainer containerDrag = objectToDrag.GetContainer();
                    if (objectToReplace)
                    {
                        containerReplace = objectToReplace.GetContainer();

                        // if there is a container and we can't drop into it for game logic reasons, cancel the drag
                        if (containerReplace != null)
                            if (containerReplace.CanDrop(objectToDrag, objectToReplace) == false)
                                objectToReplace = null;

                        // check the other way too, since the drag and drop is a swap
                        if (containerDrag != null)
                            if (containerDrag.CanDrop(objectToReplace, objectToDrag) == false)
                                objectToReplace = null;
                    }

                    if (objectToReplace != null)
                    {
                        // store the indexes of where these objects came from to pass through, they may get changed in Drop() calls
                        int dragIndex = objectToDrag.index;
                        int replaceIndex = objectToReplace.index;

                        IDraggable replacement = Instantiate(objectToReplace);
                        IDraggable current = Instantiate(objectToDrag);

                        // game logic - let both containers know about the update
                        if (containerReplace != null)
                            containerReplace.Drop(current, objectToReplace, replaceIndex, containerReplace != containerDrag);
                        if (containerDrag != null)
                            containerDrag.Drop(replacement, objectToDrag, dragIndex, true);

                        if (objectToDrag.GetContainer().DoesSwap())
                        {
                            // swap positions
                            objectToDrag.transform.position = objectToReplace.transform.position;
                            objectToReplace.transform.position = originalPosition;

                            // swap parents
                            objectToDrag.transform.parent = objectToReplace.transform.parent;
                            objectToReplace.transform.parent = originalParent;
                        }
                        else
                        {
                            // return to point of origin and nothing happens
                            objectToDrag.transform.position = originalPosition;
                            objectToDrag.transform.parent = originalParent;
                        }

                    }
                    else
                    {
                        // return to point of origin and nothing happens
                        objectToDrag.transform.position = originalPosition;
                        objectToDrag.transform.parent = originalParent;
                    }

                    objectToDragImage.raycastTarget = true;
                    objectToDrag = null;
                }

                dragging = false;
            }
        }

        // get the top GameObject  under the mouse
        private GameObject GetObjectUnderMouse()
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointer, hitObjects);
            return (hitObjects.Count <= 0) ? null : hitObjects[0].gameObject;
        }

        // get an IDraggable object under the mouse
        private IDraggable GetDraggableTransformUnderMouse()
        {
            GameObject go = GetObjectUnderMouse();

            // get top level object hit
            if (go != null)
            {
                return go.GetComponent<IDraggable>();
            }

            return null;
        }
    }

}
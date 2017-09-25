using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class SquadController : MonoBehaviour
    {
        public SquadMember[] squad;
        public GameObject memberHud;
        public GameObject[] memberHuds;

        GameObject[] moveReticle;

        float basePosY;
        float selectedPosY;

        KeyCode lastKeyPressed;
        float lastKeyTimer;
        int lastKeyCounter;

        CameraSquad cameraSquad;

        // Use this for initialization
        void Start()
        {
            if (squad.Length == 0)
                squad = FindObjectsOfType<SquadMember>();

            basePosY = memberHud.transform.position.y;
            selectedPosY = basePosY + 16;

            memberHuds = new GameObject[squad.Length];
            for (int i = 0; i < squad.Length; i++)
            {
                GameObject hud = Instantiate(memberHud);
                hud.transform.parent = memberHud.transform.parent;
                hud.transform.position = memberHud.transform.position + Vector3.right * i * 64;
                hud.name = "SquadMember" + (i + 1);
                memberHuds[i] = hud;
                squad[i].index = i;
                // select the first member by default
                squad[i].selected = (i == 0);
            }
            memberHud.SetActive(false);

            cameraSquad = Camera.main.GetComponent<CameraSquad>();
            cameraSquad.target = squad[0].transform;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSelection();
            UpdateKeyCounter();
            CheckCommands();
        }

        // update who's selected by 1,2,3,4, etc keys
        // single click toggles selection state
        // double click selects exclusively and sets character to be tracked
        void UpdateSelection()
        {
            // if we click on a character, toggle their selected state
            for (int i = 0; i < squad.Length; i++)
            {
                KeyCode kcode = KeyCode.Alpha1 + i;
                if (Input.GetKeyUp(kcode))
                {
                    if (lastKeyPressed == kcode)
                        lastKeyCounter++;

                    if (lastKeyCounter == 0)
                    {
                        squad[i].selected = !squad[i].selected;
                    }
                    else
                    {
                        ClearSelection();
                        squad[i].selected = true;
                        cameraSquad.target = squad[i].transform;
                    }

                    lastKeyPressed = kcode;
                    lastKeyTimer = 1.0f;
                }
                Vector3 pos = memberHuds[i].transform.position;
                pos.y = squad[i].selected ? selectedPosY : basePosY;
                memberHuds[i].transform.position = pos;
            }

            if (Input.GetKeyUp(KeyCode.Alpha1 + squad.Length))
                ClearSelection(true);
        }

        // update multiple keypress detection
        void UpdateKeyCounter()
        {
            if (lastKeyTimer > 0)
            {
                lastKeyTimer -= Time.deltaTime;
                if (lastKeyTimer < 0)
                {
                    lastKeyPressed = 0;
                    lastKeyCounter = 0;
                }
            }

        }

        // mopve selected characters to point of a single mouseclick
        void CheckCommands()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit info;
                if (Physics.Raycast(ray, out info))
                {
                    MoveSquadToPoint(info.point);
                }
            }
        }

        void ClearSelection(bool select = false)
        {
            foreach (SquadMember sm in squad)
                sm.selected = select;
        }

        // move the selected characters around a given point, spacing them in a circle
        void MoveSquadToPoint(Vector3 pos)
        {
            Vector3 centroid = Vector3.zero;
            List<SquadMember> selected = new List<SquadMember>();

            foreach (SquadMember sm in squad)
            {
                if (sm.selected)
                {
                    centroid += sm.transform.position;
                    selected.Add(sm);
                }
            }

            // we now have a list of all selected characters and a central point
            // if theres one or less, move them to the point we clicked
            if (selected.Count <= 1)
            {
                foreach (SquadMember sm in selected)
                {
                    sm.MoveTo(pos);
                }
            }
            else
            {
                // for two or more, sort in order of  angle from the centroid, so we can arrange them around the circle
                centroid /= (float)selected.Count;
                foreach (SquadMember sm in selected)
                {
                    sm.SetDeltaPos(sm.transform.position - centroid);
                }
                selected.Sort(delegate (SquadMember a, SquadMember b)
                {
                    return a.deltaAngle.CompareTo(b.deltaAngle);
                });
                float[] angles = new float[selected.Count];
                for (int i = 0; i < selected.Count; i++)
                    angles[i] = selected[i].deltaAngle;

                SpreadAngles(angles);

                int index = 0;
                for (int i = 0; i < selected.Count; i++)
                {
                    selected[i].MoveTo(pos + new Vector3(Mathf.Cos(angles[i]), 0, Mathf.Sin(angles[i])) * 2);
                    index++;
                }
            }






        }

        // given a set of angles, find the best even spread round a circle that minimises change.
        // assumes the angles are all sorted
        public static void SpreadAngles(float[] angles)
        {
            int num = angles.Length;

            // try each angle as the fixed one, and vary the others around it to be evenly spread
            // for each solution calculate the sum of deltaAngle^2 for all the displaced points
            // and pick the solution with the least disturbance
            float[] cost = new float[num];
            float lowestDelta = 0;
            int bestIndex = -1;

            // the increment between all the final angles - divide a circle by num
            float dAngle = 2 * Mathf.PI / (float)num;
            for (int i = 0; i < num; i++)
            {
                float delta = 0;
                for (int j = 0; j < num; j++)
                {
                    float angle = angles[i] + (j - i) * delta;
                    float da = Mathf.DeltaAngle(angle, angles[j]);
                    delta += da * da;
                }
                if (bestIndex == -1 || delta < lowestDelta)
                {
                    lowestDelta = delta;
                    bestIndex = i;
                }
            }

            // apply the best solution by keeping bestIndex fixed and space around them
            float baseAngle = angles[bestIndex];
            for (int j = 0; j < num; j++)
            {
                angles[j] = baseAngle + (j - bestIndex) * dAngle;
            }

        }
    }
}
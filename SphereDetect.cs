using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDetect : MonoBehaviour {

	public VisualizationManager visman;

	public DETECTTYPE DetectType;

	public enum DETECTTYPE
	{
		spheric,
		gw2D,
		micro2D
	}

	public VisualizationManager.Node nodeStart;

	public VisualizationManager.Node nodeEnd;

	public Transform physfront;

	public Transform physend;
	
	// Use this for initialization
	void Start () {
		visman = Camera.main.gameObject.GetComponent<VisualizationManager>();
	}

	// void OnTriggerEnter()
	// {
	// 	Debug.Log("dsf");
	// }
	// void OnTriggerStay()
	// {
	// 	Debug.Log("ddasf");
	// }

	void OnTriggerEnter (Collider nodee)
	{
		if(DetectType == 0)
		{
			
		}
		else if(DetectType == DETECTTYPE.gw2D)
		{
			if(nodee.gameObject == visman.nodesSorted[0].node && visman.pointNode.node != nodee.gameObject)
			{
				visman.CHRotating = false;
				visman.GWPointer2D.SetActive(false);
				//print(visman.convexpointLocs.Count);
				visman.realcentroid = GameObject.Instantiate(visman.centroidgo, visman.CentroidCalc(visman.convexpointLocs), Quaternion.identity);
				visman.GeneratingNetSense = true;
			}
			else if(nodee.gameObject != visman.pointNode.node)
			{
				visman.pointNode = visman.nodes[int.Parse(nodee.gameObject.name)];
				visman.pointNode.node = nodee.gameObject;
				visman.pointNode.SortID = visman.nodesSorted.Count;
				visman.GWPointer2D.transform.position = nodee.transform.position;
				visman.nodesSorted.Add(visman.nodes[int.Parse(nodee.gameObject.name)]);
				visman.pointNode.activation = VisualizationManager.ACTIVATION.sorted;
				visman.pointNode.node.GetComponent<MeshRenderer>().material = visman.stage1mat;
				visman.convexpointLocs.Add(visman.pointNode.nodeLoc);
			}
		}
		else if(DetectType == DETECTTYPE.micro2D)
		{
			VisualizationManager.Node nodereal = visman.nodes[int.Parse(nodee.gameObject.name)];
			if(nodereal.activation == VisualizationManager.ACTIVATION.norm)
			{
				nodereal.activation = VisualizationManager.ACTIVATION.sorted;
				nodereal.node.GetComponent<MeshRenderer>().material = visman.stage1mat;

				//   ||||||||||
				//   ||||||||||
				//   ||||||||||
				//   ||||||||||
				//   \\\\\/////
				//    \\\\////
				//     \\\///
				//      \\//
				//       \/
				nodereal.SortID = nodeEnd.SortID;
				visman.nodesSorted.Insert(nodeStart.SortID+1,nodereal);
				if(nodereal.SortID != visman.nodesSorted.Count-1)
				{
					for(int i = nodereal.SortID+1; i < visman.nodesSorted.Count; i++)
					{
						visman.nodesSorted[i].SortID++;
					}
				}
				//     /\
				      //\\
				     ///\\\
				    ////\\\\
				   /////\\\\\
                  //////\\\\\\
				 ///////\\\\\\\
				////////\\\\\\\\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\
				//|||||||||||||\

				//visman.printSortedIDS = true;
				//Debug.Break();
				//redistribute microsensor

				Vector3 goloc =  new Vector3(physfront.position.x + nodereal.nodeLoc.x, physfront.position.y + nodereal.nodeLoc.y, physfront.position.z + nodereal.nodeLoc.z) / 2.0000000f;
				Vector3 goscale = new Vector3(.05f,.05f, Vector3.Distance(physfront.position,nodereal.nodeLoc));
				GameObject go = GameObject.Instantiate(visman.microSensor, goloc,Quaternion.identity);
				go.transform.localScale = goscale;
				go.transform.LookAt(physfront.transform);
				SphereDetect sd = go.GetComponent<SphereDetect>();
				sd.visman = visman;
				sd.nodeStart = nodeStart;
				sd.nodeEnd = nodereal;
				go.transform.parent = visman.realcentroid.transform;
				//
				goloc =  new Vector3(physend.position.x + nodereal.nodeLoc.x, physend.position.y + nodereal.nodeLoc.y, physend.position.z + nodereal.nodeLoc.z) / 2.0000000f;
				goscale = new Vector3(.05f,.05f, Vector3.Distance(physend.position,nodereal.nodeLoc));
				go = GameObject.Instantiate(visman.microSensor, goloc,Quaternion.identity);
				go.transform.localScale = goscale;
				go.transform.LookAt(physfront.transform);
				sd = go.GetComponent<SphereDetect>();
				sd.visman = visman;
				sd.nodeStart = nodereal;
				sd.nodeEnd = nodeEnd;
				go.transform.parent = visman.realcentroid.transform;
				//
				gameObject.SetActive(false);
				visman.ClearPath();
				visman.DrawPath();
			}
		}
	}

	// void FixedUpdate()
	// {
	// 	if(DetectType == 0)
	// 	{

	// 	}
	// 	else if (DetectType == DETECTTYPE.gw2D)
	// 	{

	// 	}
	// 	else if (DetectType == DETECTTYPE.micro2D)
	// 	{
	// 		transform.position = new Vector3(nodeStart.nodeLoc.x + nodeEnd.nodeLoc.x, nodeStart.nodeLoc.y + nodeEnd.nodeLoc.y,nodeStart.nodeLoc.z + nodeEnd.nodeLoc.z) / 2f;
	// 		transform.localScale = new Vector3(.1f,.1f, Vector3.Distance(nodeStart.nodeLoc,nodeEnd.nodeLoc));
	// 	}
	// }

	void OnTriggerExit (Collider nodee)
	{
		if(DetectType == 0)
		{
			visman.initialShrink = false;
			for(int i = 0; i < visman.nodes.Count; i++)
			{
				if(visman.nodes[i].node == nodee.gameObject)
				{
					visman.furthestnode = visman.nodes[i];
					//Debug.Log("Found: "+ visman.furthestnode.nodeLoc.x);
					
				}
			}
		}
		else if(DetectType == DETECTTYPE.gw2D)
		{

		}
	}
}

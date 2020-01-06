using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class VisualizationManager : MonoBehaviour {

	public GameObject nodego;

	public Transform pen;

	public float step;

	public GameObject centergo;

	public GameObject centroidgo;

	public float yrange = 10;

	public float xrange = 10;

	public float zrange = 0;

	public bool spheric;

	public List<Node> queueq;

	public PFA pfa = 0;

	public int penindex = 0;

	public enum PFA
	{
		random,
		brute,
		BMO
	}

	public CHA cha = 0;

	public enum CHA
	{
		BMO,
		DaC,
		QHull
	}

	public class Node
	{
		public int ID = 0;
		public int SortID = 0;
		public GameObject node = null;
		public Vector3 nodeLoc = new Vector3(0,0,0);
		public ACTIVATION activation;
	}
	public enum ACTIVATION
	{
		norm,
		sorted,
		stage1,
		stage2
	}
	public List<Node> nodes = new List<Node>();
	public List<Node> nodesSorted = new List<Node>();
	public List<double> xlocations = new List<double>();
	public List<double> ylocations = new List<double>();
	public List<double> zlocations = new List<double>();
	public Vector3 center = new Vector3 (0,0,0);
	public bool spawnedcenter = false;
	public GameObject realcenter  = null;
	public GameObject realcentroid = null;
	public float calculatedDistance = 0;
	public GameObject SphericSensor;
	public bool initialShrink = false;
	public float initialShrinkSpeed = 1.0f;
	public float initialShrinkSpeed2D = 1.0f;
	public Node furthestnode = null;
	public bool fill = false;
	public Node farxnode;
	public Node farynode;
	public GameObject GWPointer2D;
	public Material normmat;
	public Material sortedmat;
	public Material stage1mat;
	public Material stage2mat;
	public Color norm;
	public Color sorted;
	public Color stage1;
	public Color stage2;
	public float CHrotSpeed = .1f;
	public bool CHRotating = false;
	public Node pointNode = null;
	public bool GeneratingNetSense = false;
	public bool AddingInterior = false;
	public GameObject microSensor;
	public List<Vector3> convexpointLocs = new List<Vector3>();
	public bool printSortedIDS;
	// Use this for initialization
	void Start () {
		normmat.color = norm;
		sortedmat.color = sorted;
		stage1mat.color = stage1;
		stage2mat.color = stage2;
	}
	
	// Update is called once per frame
	void Update () {
		if(printSortedIDS == true)
		{
			foreach(Node nod in nodesSorted)
			{
				print(nod.SortID);
			}
			printSortedIDS = false;
		}
	}
	public void Generate(int amount)
	{
		if(amount <= 0)
		{
			return;
		}
		for(int i = 0; i < amount; i++)
		{
			Node inode = new Node();
			inode.ID = i;
			if(spheric == true)
			{
				// var u = Math.random();
				// var v = Math.random();
				// var theta = u * 2.0 * Math.PI;
				// var phi = Math.acos(2.0 * v - 1.0);
				// var r = Math.cbrt(Math.random());
				// var sinTheta = Math.sin(theta);
				// var cosTheta = Math.cos(theta);
				// var sinPhi = Math.sin(phi);
				// var cosPhi = Math.cos(phi);
				// var x = r * sinPhi * cosTheta;
				// var y = r * sinPhi * sinTheta;
				// var z = r * cosPhi;
				// return {x: x, y: y, z: z};
				float cx = UnityEngine.Random.Range(-0.999999f, 0.999999f);
				float cy = UnityEngine.Random.Range(-1.0000000f, 1.00000f);
				float cz = UnityEngine.Random.Range(-1.0000000f, 1.000000f);
				float theta = 2 * (float)Math.PI * cx;
				//if(cy < 0)
				//{
				//	cy = -cy;
				//}
				if(cy < 0 ||Mathf.Abs(cy)>=1)
				{
					cy = UnityEngine.Random.Range(0.00001f, 0.99999f);
				}
				float phi = (float)Math.Acos(2*cy-1);
				//Debug.Log("PHI IS: " + phi);
				//float r = (0, Math.Sqrt(cx**2+yrange**2+zrange**2));
				//int dir = Random.Range();
				cx = xrange*(float)Math.Sin(phi)*(float)Math.Cos(theta);
				//if(cx < 0)
				//{
				//	cx = -cx;
				//}
				cy = yrange*(float)Math.Sin(phi)*(float)Math.Sin(theta);
				//if(cy < 0)
				//{
				//	cy = -cy;
				//}
				cz = zrange*(float)Math.Cos(phi);
				//if(cz < 0)
				//{
				//	cz = -cz;
				//}
				//Debug.Log("cz is:" + cz + "cy is: " + cy + "cz is " + cz);
				if(fill == true)
				{
					//cx = cx*UnityEngine.Random.Range(0.0000f, 1.0000f);
					//cy = cy*UnityEngine.Random.Range(0.0000f, 1.0000f);
					//cz = cz*UnityEngine.Random.Range(0.0000f, 1.0000f);
					float r = UnityEngine.Random.Range(0.00000f,1.00000f);
					r = 1-(r*r*r);
					r = 1.30000000f*r;
					cx=r*cx;
					cy=r*cy;
					cz=r*cz;


				}
				inode.nodeLoc = new Vector3 (cx,cy, cz);
			}
			else
			{
				if(fill == false)
				{
					inode.nodeLoc = new Vector3 (UnityEngine.Random.Range(xrange, -xrange), UnityEngine.Random.Range(yrange, -yrange), UnityEngine.Random.Range(zrange, -zrange));
				}
				else
				{
					inode.nodeLoc = new Vector3 (UnityEngine.Random.Range(xrange, -xrange), UnityEngine.Random.Range(yrange, -yrange), UnityEngine.Random.Range(zrange, -zrange));
				}
			}
			inode.node = GameObject.Instantiate(nodego, inode.nodeLoc, Quaternion.identity);
			inode.node.name = i.ToString();
			nodes.Add(inode);
			xlocations.Add(inode.nodeLoc.x);
			ylocations.Add(inode.nodeLoc.y);
			zlocations.Add(inode.nodeLoc.z);
			if(farxnode == null || inode.nodeLoc.x > farxnode.nodeLoc.x)
			{
				farxnode = inode;
			}
			if(farynode == null || inode.nodeLoc.y > farynode.nodeLoc.y)
			{
				farynode = inode;
			}
			//distances.Add(new Vector3(inode.x, inode.y, inode.z));
		}
		center = new Vector3((float)xlocations.Average(),(float)ylocations.Average(),(float)zlocations.Average());
		if(spawnedcenter == false)
		{
			realcenter = GameObject.Instantiate(centergo, center, Quaternion.identity);
		}
		else
		{
			realcenter.transform.position = center;
		}
		foreach(Node n in nodes)
		{
			//while(pen.position != n.nodeLoc)
			//{
			//	pen.position = Vector3.MoveTowards(pen.position, n.nodeLoc, step);
			//}
		}
	}

	public void SortNodes()
	{
		if(pfa == 0)
		{
			nodesSorted = nodes;
			for (int i = nodes.Count - 1; i > 0; i--)
             {
                 // Randomize a number between 0 and i (so that the range decreases each time)
                 int rnd = UnityEngine.Random.Range(0, i);
     
                 // Save the value of the current i, otherwise it'll overwrite when we swap the values
                 Node temp = nodesSorted[i];
     
                 // Swap the new and old values
                 nodesSorted[i] = nodesSorted[rnd];
                 nodesSorted[rnd] = temp;
             }
		}
		else if(pfa == PFA.BMO)
		{
			
			if(xrange == 0 || yrange == 0 || zrange == 0)
			{
				ConvexHull2D();
			}
			else
			{
				ConvexHull3D();
			}
		}
	}

	public void FixedUpdate()
	{
		if(penindex != nodesSorted.Count-1 && nodesSorted.Count > 0 && penindex >= 0)
		{
			Time.timeScale = 0.5f;
			pen.position = nodesSorted[penindex].nodeLoc;
			penindex++;
		}
		else if(penindex == nodesSorted.Count-1)
		{
			pen.position = nodesSorted[penindex].nodeLoc;
			penindex = -2;
		}
		else if(penindex == -2 && nodesSorted.Count > 0 && CHRotating == false && GeneratingNetSense == false && AddingInterior == false)
		{
			pen.position = nodesSorted[0].nodeLoc;
		}
		else if(initialShrink == true && SphericSensor.transform.localScale.x > 0.1f)
		{
			Time.timeScale = 1.0f;
			SphericSensor.transform.localScale = Vector3.Lerp(SphericSensor.transform.localScale, new Vector3(0.05f, 0.05f, 0.05f), initialShrinkSpeed);
		}
		else if(CHRotating == true)
		{
			Time.timeScale = 1.0f;
			GWPointer2D.transform.Rotate(CHrotSpeed, 0.0f, 0.0f, Space.Self);
		}
		else if(GeneratingNetSense == true)
		{
			Time.timeScale = 1.0f;
			int ix = 0;
			foreach(Node Sortednode in nodesSorted)
			{
				Node nodeNext;
				if(ix != nodesSorted.Count-1)
				{
					nodeNext = nodesSorted[ix + 1];
				}
				else
				{
					nodeNext = nodesSorted[0];
				}
				Vector3 goloc =  new Vector3(Sortednode.nodeLoc.x + nodeNext.nodeLoc.x, Sortednode.nodeLoc.y + nodeNext.nodeLoc.y, Sortednode.nodeLoc.z + nodeNext.nodeLoc.z) / 2f;
				Vector3 goscale = new Vector3(.05f,.05f, Vector3.Distance(Sortednode.nodeLoc,nodeNext.nodeLoc));
				GameObject go = GameObject.Instantiate(microSensor, goloc,Quaternion.identity);
				go.transform.localScale = goscale;
				go.transform.LookAt(Sortednode.node.transform);
				SphereDetect sd = go.GetComponent<SphereDetect>();
				sd.visman = this;
				sd.nodeStart = Sortednode;
				sd.nodeEnd = nodeNext;
				go.transform.parent = realcentroid.transform;
				ix++;
			}
			GeneratingNetSense = false;
			AddingInterior = true;
		}
		else if(AddingInterior == true && centroidgo.transform.localScale.x >= 0.0003f && nodesSorted.Count != nodes.Count)
		{
			Time.timeScale = 1.0f;
			realcentroid.transform.localScale = Vector3.Lerp(realcentroid.transform.localScale, new Vector3(0.0001f, 0.0001f, 0.0001f), initialShrinkSpeed2D);
		}
		else
		{
			Time.timeScale = 1.0f;
			initialShrink = false;
			AddingInterior = false;
			if(nodesSorted.Count == nodes.Count && nodesSorted.Count > 0)
			{
				foreach(Node finalnode in nodesSorted)
				{
					finalnode.activation = ACTIVATION.sorted;
					finalnode.node.GetComponent<MeshRenderer>().material = sortedmat;
				}
				realcentroid.SetActive(false);
			}
		}
	}

	public Vector3 CentroidCalc (List<Vector3> convexPoints)
	{
		List<float> Areas = new List<float>();
		float totalarea = 0.0f;
		List<Vector3> tricenters = new List<Vector3>();
		Vector3 centroid = Vector3.zero;
		int verts = convexPoints.Count;
		for(int i = 0; i < verts - 2; i++)
		{
			float a = (convexPoints[i+1].x*convexPoints[0].y)-(convexPoints[i+2].x*convexPoints[0].y)-(convexPoints[0].x*convexPoints[i+1].y)+(convexPoints[i+2].x*convexPoints[i+1].y)+(convexPoints[0].x*convexPoints[i+2].y)-(convexPoints[i+1].x*convexPoints[i+2].y);
			float b = (convexPoints[i+1].x*convexPoints[0].z)-(convexPoints[i+2].x*convexPoints[0].z)-(convexPoints[0].x*convexPoints[i+1].z)+(convexPoints[i+2].x*convexPoints[i+1].z)+(convexPoints[0].x*convexPoints[i+2].z)-(convexPoints[i+1].x*convexPoints[i+2].z);
			float c = (convexPoints[i+1].y*convexPoints[0].z)-(convexPoints[i+2].y*convexPoints[0].z)-(convexPoints[0].y*convexPoints[i+1].z)+(convexPoints[i+2].y*convexPoints[i+1].z)+(convexPoints[0].y*convexPoints[i+2].z)-(convexPoints[i+1].y*convexPoints[i+2].z);
			//float tria = Math.Abs((convexPoints[0].x*(convexPoints[i+1].y-convexPoints[i+2].y)+convexPoints[i+1].x*(convexPoints[i+2].y-convexPoints[0].y)+convexPoints[i+2].x*(convexPoints[0].y-convexPoints[i+1].y))/2.0000000000f);
			float tria = (float)Math.Sqrt((a*a)+(b*b)+(c*c))/2.00000f;
			Vector3 triloc = (convexPoints[0]+convexPoints[i+1]+convexPoints[i+2])/3.0000000000f;
			totalarea += tria;
			Areas.Add(tria);
			tricenters.Add(triloc);
			centroid += triloc*tria;
			//print("IN: " + i + ", TRIA: " + tria + ", TriLoc: " + triloc);
		}
		centroid = centroid/totalarea;
		return centroid;
	}

	public void ConvexHull2D()
	{
		if(cha == 0)
		{
			if(xrange != 0)
			{
				pointNode = farxnode;
				GWPointer2D.transform.position = farxnode.nodeLoc;
				if(zrange == 0)
				{
					GWPointer2D.transform.eulerAngles = new Vector3(0.0f,90.0f, 0.0f);
				}
				if(yrange == 0)
				{
					GWPointer2D.transform.eulerAngles = new Vector3(0.0f,90.0f, 90.0f);
				}
			}
			else
			{
				pointNode = farynode;
				GWPointer2D.transform.position = farynode.nodeLoc;
			}
			GWPointer2D.SetActive(true);
			nodesSorted.Add(pointNode);
			convexpointLocs.Add(pointNode.nodeLoc);
			pointNode.activation = ACTIVATION.sorted;
			pointNode.node.GetComponent<MeshRenderer>().material = sortedmat;
			pointNode.SortID = 0;
			CHRotating = true;
			
		}
	}

	public void ConvexHull3D()
	{
		if(SphericSensor.activeInHierarchy == false)
		{
			SphericSensor.SetActive(true);
			SphericSensor.transform.position = centergo.transform.position;
		}
		initialShrink = true;
	}

	public void DrawPath()
	{
		Time.timeScale = 0.5f;
		pen.gameObject.GetComponent<TrailRenderer>().enabled = false;
		pen.gameObject.GetComponent<TrailRenderer>().time = 0;
		pen.position = nodesSorted[0].nodeLoc;
		pen.gameObject.GetComponent<TrailRenderer>().enabled = true;
		pen.gameObject.GetComponent<TrailRenderer>().time = 25.5f;
		penindex = 0;
		//for (int i = nodesSorted.Count;i < 0; i++)
		//{
		//	pen.position = nodesSorted[i].nodeLoc;
		//}
		//pen.position = nodesSorted[0].nodeLoc;
		Time.timeScale = 1.0f;
	}
	public void ClearPath()
	{
		pen.gameObject.GetComponent<TrailRenderer>().enabled = false;
		pen.gameObject.GetComponent<TrailRenderer>().time = 0;
		pen.position = nodesSorted[0].nodeLoc;
		pen.gameObject.GetComponent<TrailRenderer>().enabled = true;
		pen.gameObject.GetComponent<TrailRenderer>().time = 25.5f;
	}
}

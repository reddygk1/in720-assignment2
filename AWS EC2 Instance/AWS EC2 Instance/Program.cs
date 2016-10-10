using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace AWS_EC2_Instance
{
    class Program
    {
        static AmazonEC2Client ec2Client;
        static void Main(string[] args)
        {
            ec2Client = new AmazonEC2Client(RegionEndpoint.USWest1);

            string instanceId = GetInstanceId();

            Console.WriteLine("Enter the command");
            string response = Console.ReadLine();

            if (response == "start")
            {
                if (instanceId == null)
                {
                    CreateInstance();
                }
                else
                {
                    StartInstance(instanceId);
                }

            }
            if (response == "stop")
            {
                if (instanceId != null)
                {
                    StopInstance(instanceId);
                }
            }
            if (response == "terminate")
            {
                if (instanceId != null)
                {
                    TermnateInstance(instanceId);
                }

            }

            if (response == "status")
            {
                if (instanceId != null)
                {
                    //InstanceStatus(instanceId);
                    Console.WriteLine(InstanceStatus(instanceId));
                }

            }

            Console.ReadLine();
        }

        static void StartInstance(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            StartInstancesRequest startInstanceRequest = new StartInstancesRequest(instanceIds);
            ec2Client.StartInstances(startInstanceRequest);

        }
        static void StopInstance(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            StopInstancesRequest stopInstanceRequest = new StopInstancesRequest(instanceIds);
            ec2Client.StopInstances(stopInstanceRequest);

        }
        static void TermnateInstance(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            TerminateInstancesRequest terminateInstanceRequest = new TerminateInstancesRequest(instanceIds);
            ec2Client.TerminateInstances(terminateInstanceRequest);

        }

        static string InstanceStatus(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            DescribeInstanceStatusRequest instanceStatusRequest = new DescribeInstanceStatusRequest();
            instanceStatusRequest.InstanceIds = instanceIds;
            instanceStatusRequest.IncludeAllInstances = true;

            DescribeInstanceStatusResponse instanceStatusResponse = ec2Client.DescribeInstanceStatus(instanceStatusRequest);

            string state = instanceStatusResponse.InstanceStatuses[0].InstanceState.Name;


            return state;

        }


        static DescribeInstancesResponse getInstanceResponse()
        {
            DescribeInstancesRequest instanceRequest = new DescribeInstancesRequest();
            List<Filter> filters = new List<Filter>();
            List<String> tagFilter = new List<String>() { "Reddy" };
            List<String> stateFilter = new List<String>() { "pending", "running", "shutting-down", "stopping", "stopped" };
            filters.Add(new Filter("tag-value", tagFilter));
            filters.Add(new Filter("instance-state-name", stateFilter));
            instanceRequest.Filters = filters;

            DescribeInstancesResponse response = ec2Client.DescribeInstances(instanceRequest);
            return response;

        }

        static void CreateInstance()
        {
            RunInstancesRequest runInstance = new RunInstancesRequest("ami-31490d51", 1, 1);
            runInstance.InstanceType = InstanceType.T2Nano;

            RunInstancesResponse runInstanceResponse = ec2Client.RunInstances(runInstance);

            string instanceId = runInstanceResponse.Reservation.Instances[0].InstanceId;

            List<string> instanceList = new List<string>() { instanceId };
            List<Tag> tagList = new List<Tag>();
            tagList.Add(new Tag("Name", "Reddy"));
            CreateTagsRequest tagRequest = new CreateTagsRequest(instanceList, tagList);

            ec2Client.CreateTags(tagRequest);


        }
        static string GetInstanceId()
        {
            string instanceID = null;
            DescribeInstancesResponse response = getInstanceResponse();
            if (response.Reservations.Count > 0)
            {
                instanceID = response.Reservations[0].Instances[0].InstanceId;

            }

            return instanceID;
        }
    }
}

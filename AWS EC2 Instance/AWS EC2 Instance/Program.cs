/*******************************************************
 *                 In720 VIRTUALISATION                *
 *              Assignment 2 (EC2 Instance)            *
 *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon;
using Amazon.EC2;
using Amazon.EC2.Model;
using System.Threading;

namespace AWS_EC2_Instance
{
    class Program
    {
        static AmazonEC2Client ec2Client;
        //Method to display welcome Screen.
        static void WelcomeScreen()
        {
            Console.WriteLine("***************************************************");
            Console.WriteLine("*            VIRTUALISATION - Assignment 2        *");
            Console.WriteLine("*            EC2 Instances through console        *");
            Console.WriteLine("***************************************************");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("The commands available to communicate with EC2 instances are as follows");
            Console.WriteLine(" ");
            Console.WriteLine("START       - To start the Istance");
            Console.WriteLine("STOP        - To stop the Instance");
            Console.WriteLine("STATUS      - To get the status of Insance");
            Console.WriteLine("TERMINATE   - To terminate the Instance");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Console.WriteLine("Please enter one of the command");

        }
      
        //Method that returns the details of instance with my last name.
        static DescribeInstancesResponse getInstanceResponse()
        {
            DescribeInstancesRequest instanceRequest = new DescribeInstancesRequest();
            List<Filter> filters = new List<Filter>();
            List<String> tagFilter = new List<String>() { "Reddy" };
            //we want the details of the instance which is created and either running or stopped. If it is terminate, we don't care.
            List<String> stateFilter = new List<String>() { "pending", "running", "shutting-down", "stopping", "stopped" };
            filters.Add(new Filter("tag-value", tagFilter));
            filters.Add(new Filter("instance-state-name", stateFilter));
            instanceRequest.Filters = filters;

            DescribeInstancesResponse response = ec2Client.DescribeInstances(instanceRequest);
            return response;

        }

        //Method that gets the Instance Id.
        static string GetInstanceId()
        {
            string instanceID = null;
            DescribeInstancesResponse response = getInstanceResponse(); //Collect the response, which gives all the info of instance with my last name.
            if (response.Reservations.Count > 0)
            {
                instanceID = response.Reservations[0].Instances[0].InstanceId; //Dig in and get Instance Id.

            }

            return instanceID;
        }
        //Method to Start the Instance.
        static void StartInstance(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            StartInstancesRequest startInstanceRequest = new StartInstancesRequest(instanceIds);
            ec2Client.StartInstances(startInstanceRequest);

        }

        //Method to stop Instance.
        static void StopInstance(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            StopInstancesRequest stopInstanceRequest = new StopInstancesRequest(instanceIds);
            ec2Client.StopInstances(stopInstanceRequest);

        }

        //Method to termonate Instance.
        static void TermnateInstance(string instanceID)
        {
            List<String> instanceIds = new List<string>() { instanceID };
            TerminateInstancesRequest terminateInstanceRequest = new TerminateInstancesRequest(instanceIds);
            ec2Client.TerminateInstances(terminateInstanceRequest);

        }

        //Method to get the status of the Instance.
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

        //method that creates a new Instance.
        static void CreateInstance()
        {
            //We pass in the image that we are going to use. It also takes 2  other arguments which are min and max count (number of instances we want to create), max 5 can be created.
            RunInstancesRequest runInstance = new RunInstancesRequest("ami-31490d51", 1, 1);
            runInstance.InstanceType = InstanceType.T2Nano;

            RunInstancesResponse runInstanceResponse = ec2Client.RunInstances(runInstance);

            string instanceId = runInstanceResponse.Reservation.Instances[0].InstanceId;
            //Setting up tage for the instance.
            List<string> instanceList = new List<string>() { instanceId };
            List<Tag> tagList = new List<Tag>();
            tagList.Add(new Tag("Name", "Reddy"));
            CreateTagsRequest tagRequest = new CreateTagsRequest(instanceList, tagList);

            ec2Client.CreateTags(tagRequest);
        }

        static void Main(string[] args)
        {
            ec2Client = new AmazonEC2Client(RegionEndpoint.USWest1);

            string instanceId = GetInstanceId();

            char ans;
            do
            {
                WelcomeScreen();
                string response = Console.ReadLine().ToLower();
                switch (response)
                {
                    case "start":
                        if (instanceId == null)
                        {
                            Console.WriteLine("There is no Instance, creating a new Instance...");
                            Thread.Sleep(2000);
                            CreateInstance();
                            Console.WriteLine("New Instance created");
                        }
                        else
                        {
                            Console.WriteLine("Starting the Instance...");
                            Thread.Sleep(2000);
                            StartInstance(instanceId);
                        }

                        break;
                    case "stop":
                        if (InstanceStatus(instanceId) == "stopped")
                        {
                            Console.WriteLine("The Instance is already stoppped");
                        }
                        else if (instanceId != null)
                        {
                            Console.WriteLine("Stopping the Instance...");
                            Thread.Sleep(2000);
                            StopInstance(instanceId);
                            Console.WriteLine("Instance Stoppped");
                        }
                        break;
                    case "status":
                        if (instanceId != null)
                        {
                            Console.WriteLine("You Instance status is " + InstanceStatus(instanceId));
                        }
                        else
                        {
                            Console.WriteLine("There is no Instance currently");
                        }
                        break;
                    case "terminate":
                        if (instanceId != null)
                        {
                            Console.WriteLine("Terminating the Instance....");
                            Thread.Sleep(2000);
                            TermnateInstance(instanceId);
                            Console.WriteLine("Instance terminated.");
                        }
                        else
                        {
                            Console.WriteLine("There is no Instance currently");
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid command!!!");
                        break;
                }
                Thread.Sleep(3000);
                Console.Clear();
                Console.WriteLine("Would you like to enter another commmand? y/n ");
                ans = Convert.ToChar(Console.ReadLine());

            } while (ans != 'n');
            Console.WriteLine("Have a good day!!!");
            Thread.Sleep(3000);
        }

    }
}

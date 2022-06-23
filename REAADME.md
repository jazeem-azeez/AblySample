# Overview

The problem is to implement a client and server that implement the following functionality.

A client connects to the server to receive a stream of numbers. Upon receiving a connection, the server streams a sequence of random numbers in a random number of discrete messages, each 1s apart, until all the messages are sent and the connection is then closed.

The client's job is to connect and receive the complete sequence of numbers, close the connection, and return the sum of the received numbers. 

The client must be capable of reconnecting and continuing to receive the sequence in the case that its connection drops. In implementing this, the client is allowed to assume that the sequence is deterministic; ie given any number in the sequence, the subsequent numbers will always be the same.

## How to build & Run 

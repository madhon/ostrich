/*
 *  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 *
 */
 
namespace Ostrich.Http
{
  using System;
  using System.Net;
  using System.Net.Sockets;
  using Ostrich.Logging;

  /// <summary>
  /// HttpServer is a simple IKayakServer implementation using `System.Net.Sockets.Socket`.
  /// 
  /// `ISocket` values are yielded on `ThreadPool` threads as determined by the Socket object. 
  /// The operations that these `ISocket` values expose yield on `ThreadPool` threads
  /// as well. Thus, you must take care to synchronize resources shared by concurrent requests.
  /// </summary>
  public class HttpServer : IKayakServer, IDisposable
  {
    private static readonly ILog logger = LogProvider.GetCurrentClassLogger();

    public IPEndPoint ListenEndPoint { get; }

    private readonly int backlog;

    private Socket listener;

    /// <summary>
    /// Constructs a server which binds to port 8080 on all interfaces upon subscription
    /// and maintains a default connection backlog count.
    /// </summary>
    public HttpServer() : this(new IPEndPoint(IPAddress.Any, 8080))
    {
    }

    /// <summary>
    /// Constructs a server which binds to the given local end point upon subscription
    /// and maintains a default connection backlog count.
    /// </summary>
    public HttpServer(IPEndPoint listenEndPoint) : this(listenEndPoint, 1000)
    {
    }

    /// <summary>
    /// Constructs a server which binds to the given local end point upon subscription
    /// and maintains the given connection backlog count.
    /// </summary>
    public HttpServer(IPEndPoint listenEndPoint, int backlog)
    {
      ListenEndPoint = listenEndPoint;
      this.backlog = backlog;
    }

    public IDisposable Start()
    {
      listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
      listener.Bind(ListenEndPoint);
      listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
      listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
      listener.Listen(backlog);

      return new Stopper(listener);
    }

    private class Stopper : IDisposable
    {
      private readonly Socket listener;

      public Stopper(Socket listener)
      {
        this.listener = listener;
      }

      public void Dispose()
      {
        listener.Close();
      }
    }

    public Action<Action<ISocket>> GetConnection()
    {
      return r => listener.BeginAccept(iasr =>
      {
        try
        {
          r(new DotNetSocket(listener.EndAccept(iasr)));
        }
        catch (ObjectDisposedException)
        {
        }
        catch (Exception e)
        {
          logger.ErrorException("GetConnection", e);
        }
      }, null);
    }

      public void Dispose()
      {
          listener?.Dispose();
      }
  }

  internal class DotNetSocket : ISocket
  {
    private readonly Socket socket;

    public DotNetSocket(Socket socket)
    {
      this.socket = socket;
    }

    public IPEndPoint RemoteEndPoint
    {
      get { return (IPEndPoint) socket.RemoteEndPoint; }
    }

    public void Dispose()
    {
      socket.Close();
    }

    public Action<Action<int>, Action<Exception>> Write(byte[] buffer, int offset, int count)
    {
      return Continuation.FromAsync<int>(
        (c, s) => socket.BeginSend(buffer, offset, count, SocketFlags.None, c, s),
        socket.EndSend);
    }

    public Action<Action, Action<Exception>> WriteFile(string file)
    {
      return Continuation.FromAsync(
        (c, s) => socket.BeginSendFile(file, c, s),
        iasr => { socket.EndSendFile(iasr); });
    }

    public Action<Action<int>, Action<Exception>> Read(byte[] buffer, int offset, int count)
    {
      return Continuation.FromAsync<int>(
        (c, s) => socket.BeginReceive(buffer, offset, count, SocketFlags.None, c, s),
        socket.EndSend);
    }
  }
}
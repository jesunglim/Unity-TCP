import socket

host = "127.0.0.1"
port = 25001

data = ""
# SOCK_STREAM means a TCP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    
try:
    # Connect to server and send data
    sock.connect((host, port))

    while True:
        str = input("input string(quit: q) : ")
        if str == "q":
            print("netSend closed")
            break
        else:
            data = str
        sock.sendall(data.encode("utf-8"))
        data = sock.recv(1024).decode("utf-8")
        print(data)

    

finally:
    sock.close()
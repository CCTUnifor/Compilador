import networkx as nx
import matplotlib.pyplot as plt

def recursiveMatPlotLib(graph, ploted, G, node, x, y):
        """internal use"""
        G.add_node(node.id, pos=(x, y))
        ploted.append(node)
        
        length = len(node.paths)
        delta = int(length/2)
        newX = x + 1
        even = length%2 == 0

        for i, path in enumerate(node.paths):
            G.add_edge(node.id, path.dest.id, weight=(path.value.value if path.value else "ɛ"))
            newY = y + (i -  delta)
            if(even and newY is y):
                newY += 1
            if(path.dest not in ploted):
                recursiveMatPlotLib(graph, ploted, G, path.dest, newX, newY)


def printMatplotlib(graph):
    """open a window with the informed graph"""
    G=nx.Graph()

    recursiveMatPlotLib(graph, [], G, graph.root, 1, 10)

    pos = nx.get_node_attributes(G,'pos')
    nx.draw(G,pos, with_labels=True)

    labels = nx.get_edge_attributes(G,'weight')
    nx.draw_networkx_edge_labels(G, pos, edge_labels=labels)
    plt.show()

def Grammar_Printer(g):
    print('-----------------------GRAMÁTICA-----------------------')
    print(g)

    print('-------------------------FIRST-------------------------')
    for term in g.Premises:
        print(term.strFirst())

    print('\n------------------------FOLLOW-------------------------')
    for term in g.Premises:
        print(term.strFollow())
    
def Grammar_Table_Printer(s):
    ljust = 15
    # print('\n-------------------------TABLE-------------------------')
    # print(s.table)
    print()

    # header
    for non_terminal in s.table:
        line = "".ljust(ljust + len(non_terminal) - 1)

        for terminal in s.table[non_terminal]:
            line += terminal.ljust(ljust)
        
        centerWord = "TABLE"
        
        lineLen = len(line)
        halfLineLen = int(lineLen/2)
        centerWordLen = len(centerWord)
        halfCenterWordLen = int(centerWordLen/2)

        headerLines = "".ljust((halfLineLen - halfCenterWordLen), '-')
        print(headerLines + centerWord + headerLines)
        print("|" + line + "|")

        break

    # body
    for non_terminal in s.table:
        line = non_terminal.ljust(ljust)

        for terminal in s.table[non_terminal]:
            value = s.table[non_terminal][terminal]
            if(type(value) is tuple):
                line += str(value[1]).ljust(ljust)
            else:
                line += str(value).ljust(ljust)
        
        print("|" + line + "|")

def LexicPrint(tokens):
    print('\n-------------------------LEXIC-------------------------')
    
    for token in tokens:
        print(token)

def CompileHistoric(historic):
    print('\n-----------------------HISTORIC-----------------------')
    print(historic)
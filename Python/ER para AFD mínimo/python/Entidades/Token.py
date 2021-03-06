class TokenType:
    def __init__(self, ttype, values, name, Terminal=False):
        self.id = ttype
        self.values = values
        self.name = name
        self.terminal = Terminal


class Token:
    NUMBER = TokenType(1, None, 'Number', True)
    OPERATOR = TokenType(2, ['+', '*'], 'Operator')
    PARENTHESES = TokenType(3, ['(', ')'], 'Parentheses')
    WORD = TokenType(4, None, 'Word', True)
    COMMENT = TokenType(5, ['-'], 'Comment')
    OR = TokenType(5, ['|'], 'Or')

    def __init__(self, value, ttype):
        self.value = value
        self.ttype = ttype
    
    def __str__(self):
        return "'" + str(self.value) + "' " + self.ttype.name

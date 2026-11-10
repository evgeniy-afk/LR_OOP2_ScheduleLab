<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="html" encoding="UTF-8" indent="yes"/>

  <xsl:template match="/Schedule">
    <html>
      <head>
        <title>Розклад занять</title>
                <style>
          :root {
            --bg: #ffffff;
            --text: #1f1f1f;
            --muted: #555555;
            --title: #2b4a8b;
            --border: #9aa5b8;
            --head-bg: #2b4a8b;
            --head-text: #ffffff;
            --row-bg: #ffffff;
            --row-alt-bg: #eef2fa;
          }
          @media (prefers-color-scheme: dark) {
            :root {
              --bg: #1e1e1e;
              --text: #e8e8e8;
              --muted: #b0b0b0;
              --title: #8fb0f0;
              --border: #4a5568;
              --head-bg: #2b4a8b;
              --head-text: #ffffff;
              --row-bg: #262626;
              --row-alt-bg: #2f3542;
            }
          }
          body {
            font-family: Segoe UI, Arial, sans-serif;
            margin: 20px;
            background: var(--bg);
            color: var(--text);
          }
          h1 { color: var(--title); }
          p b { color: var(--text); }
          table { border-collapse: collapse; width: 100%; }
          th, td {
            border: 1px solid var(--border);
            padding: 6px 8px;
            text-align: left;
            vertical-align: top;
          }
          th { background: var(--head-bg); color: var(--head-text); }
          td { background: var(--row-bg); color: var(--text); }
          tr:nth-child(even) td { background: var(--row-alt-bg); }
          .note { color: var(--muted); font-size: 12px; }
        </style>
      </head>
      <body>
        <h1>Розклад занять</h1>
        <p>
          <b>Факультет:</b> <xsl:value-of select="@faculty"/><br/>
          <b>Навчальний рік:</b> <xsl:value-of select="@academicYear"/><br/>
          <b>Кількість занять:</b> <xsl:value-of select="count(Lesson)"/>
        </p>
        <table>
          <tr>
            <th>№</th><th>День</th><th>Час</th><th>Дисципліна</th><th>Тип</th>
            <th>Викладач</th><th>Кафедра</th><th>Аудиторія</th>
            <th>Навчальний план</th><th>Група</th><th>Студенти</th><th>Примітки</th>
          </tr>
          <xsl:apply-templates select="Lesson"/>
        </table>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Lesson">
    <tr>
      <td><xsl:value-of select="@id"/></td>
      <td><xsl:value-of select="@day"/></td>
      <td><xsl:value-of select="@time"/></td>
      <td><xsl:value-of select="Subject"/></td>
      <td>
        <xsl:choose>
          <xsl:when test="@type"><xsl:value-of select="@type"/></xsl:when>
          <xsl:otherwise>—</xsl:otherwise>
        </xsl:choose>
      </td>
      <td>
        <xsl:value-of select="Teacher/FullName"/><br/>
        <span class="note"><xsl:value-of select="Teacher/Position"/></span>
      </td>
      <td><xsl:value-of select="Teacher/Department"/></td>
      <td>
        <xsl:value-of select="Classroom/@number"/> (<xsl:value-of select="Classroom/@building"/>)<br/>
        <span class="note">місць: <xsl:value-of select="Classroom/@capacity"/></span>
      </td>
      <td>
        семестр <xsl:value-of select="Curriculum/@semester"/>,
        <xsl:value-of select="Curriculum/@hours"/> год.<br/>
        <span class="note"><xsl:value-of select="Curriculum/@control"/></span>
      </td>
      <td><xsl:value-of select="Students/@group"/> (<xsl:value-of select="Students/@course"/> курс)</td>
      <td>
        <xsl:for-each select="Students/Student">
          <xsl:value-of select="."/>
          <xsl:if test="position() != last()"><br/></xsl:if>
        </xsl:for-each>
      </td>
      <td class="note">
        <xsl:if test="@week">Тиждень: <xsl:value-of select="@week"/><br/></xsl:if>
        <xsl:if test="@subgroup">Підгрупа: <xsl:value-of select="@subgroup"/><br/></xsl:if>
        <xsl:if test="@online">Онлайн: <xsl:value-of select="@online"/></xsl:if>
      </td>
    </tr>
  </xsl:template>
</xsl:stylesheet>
